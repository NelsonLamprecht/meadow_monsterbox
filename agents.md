# Agent notes: architecture and patterns

This is a Wilderness Labs Meadow F7 firmware project (Meadow.Sdk / .NET, `netstandard2.1`, runs on physical hardware — relays driving pneumatic cylinders and an MP3 player, controlled over HTTP by a companion mobile app). It shares its architecture with the sibling project `meadow_scarecrow` (same author, same hardware family). If you're building a new Meadow app, or extending either of these, follow the pattern described here rather than reinventing it — read `meadow_scarecrow`'s code too, since some pieces (e.g. `WatchdogService`) were authored there first and ported here verbatim.

## Composition root: `MeadowApp` + `MeadowBase`

`MeadowBase.cs` is a thin abstract base (`MeadowBase : App<F7FeatherV1>`) that exposes two conveniences so nothing else in the app has to reach for the static `Resolver` directly:

```csharp
protected Logger Logger { get; } = Resolver.Log;
protected ServiceCollection Services { get; } = Resolver.Services;
```

`MeadowApp : MeadowBase` is the only place that wires the app together, in a fixed lifecycle:

1. **`Initialize()`** — construct and register every controller/service, and do the *cheap, synchronous* parts of hardware setup. Two-phase construction is deliberate: `Services.Create<T>()` builds the object and resolves its constructor dependencies (see below); a separate explicit `.Initialize(...)` call on controllers that own real hardware then opens ports/peripherals. Keeping these separate means initialization order is controlled explicitly here, not implicitly by constructor side effects.
2. **`Run()`** — start anything that should begin ticking once the app is otherwise ready (here: enabling the watchdog). Returns/awaits `base.Run()`.
3. **`OnWifiConnected` (a `NetworkConnected` handler)** — anything that needs a live IP address (the Maple HTTP server) is deferred to here, not `Initialize()`, since the network isn't up yet at boot. Guarded by a `_servicesStarted` flag because `AutomaticallyReconnect` can fire this event again after a drop — don't restart the Maple server twice.
4. **`OnError` / `OnShutdown`** — log unhandled errors; dispose any controller that implements `IDisposable` (checked via `Services.ContainsRegisteredType<T>()` first, since shutdown can be reached before those services were ever created).

WiFi connection itself is **not** hand-rolled — `meadow.config.yaml`'s `Coprocessor.AutomaticallyStartNetwork`/`AutomaticallyReconnect` plus a gitignored `wifi.config.yaml` (credentials) handle it. `MeadowApp` only listens for the `NetworkConnected` event; it never calls `Connect()` itself.

## Controllers vs. Services

- **`Controllers/`** = thin hardware drivers (`RelayController`, `MP3Controller`, `LedController`). Each owns one piece of physical hardware and exposes simple imperative methods (`TurnOnLeft()`, `PlayFile(...)`, `SetColor(...)`).
- **`Services/`** = app-level concerns that aren't tied to one piece of hardware (`MapleService` — the HTTP server; `NetworkService` — post-connect diagnostics; `DiagnosticsService` — device/OS/NTP/WiFi info logging; `Watchdog/WatchdogService` — hang recovery).
- `CylindersController` sits one level up from `RelayController` — it's still in `Controllers/` because it's still hardware-shaped (shake orchestration), but note it depends on another controller (`RelayController`) via constructor injection, not on `IMeadowDevice` directly. That's fine — dependencies compose.

Both layers share the same shape: `Controllers/BaseController.cs` and `Services/BaseService.cs` are near-identical — constructor takes a `Logger`, exposes it as a property, and a `virtual Task Run()` that defaults to a no-op. `Run()` is only overridden by services that need to actually do something at start (`MapleService` builds and starts the server; `WatchdogService` doesn't override it — its work happens via explicit `Enable()`/`Pet()` calls instead). Most services (`DiagnosticsService`, `NetworkService`) never have `Run()` called at all — they're just DI-registered helpers invoked via their own named methods.

## Real constructor DI via `Resolver.Services` (`ServiceCollection`)

Every controller/service constructor declares what it needs, and `Services.Create<T>()` resolves and injects it — no controller or service reaches into `Resolver.Services.Get<T>()` from inside its own business logic. Patterns in use:

- `Services.Create<T>()` — construct `T`, resolving constructor params against already-registered services, and register it.
- `Services.Create<TImpl, TInterface>()` — same, but register it under an interface (e.g. `Services.Create<WatchdogService, IWatchdogService>()`), so later code depends on the abstraction (`Services.Get<IWatchdogService>()`), not the concrete type.
- `Services.Add(instance)` — register an already-constructed object (used for the `IWiFiNetworkAdapter` obtained from `Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>()`, which isn't constructed by us).
- `Services.Get<T>()` — retrieve a previously registered instance.
- `Services.ContainsRegisteredType<T>()` — check before `Get<T>()` when the caller might run before that type was ever registered (shutdown paths).

**Ordering matters**: register a dependency before anything that needs it. E.g. `RelayController` is `Create`d before `CylindersController` (which takes `RelayController` in its constructor); the WiFi adapter is `Add`ed before `MapleService`/`NetworkService` are `Create`d (both take `INetworkAdapter`).

**The one sanctioned exception**: `ControllerRequestHandler : RequestHandlerBase` (the Maple HTTP route handler) is instantiated by the Maple framework itself, outside this DI container — it has no way to receive constructor-injected dependencies. It correctly reaches into the global registry directly (`Resolver.Services.Get<MP3Controller>()`, `Resolver.Services.Get<CylindersController>()`). Don't "fix" this to use constructor injection; it can't.

## The `IMeadowDevice` vs. concrete device-type split

Controllers take `IMeadowDevice device` via constructor injection — that interface covers the common surface (`CreateDigitalOutputPort`, `PlatformOS`, `Information`, etc.). But **named onboard pins** (`OnboardLedRed`, `OnboardLedGreen`, `OnboardLedBlue`) only exist on the concrete `F7FeatherV1` type's `Pins` property, not on the generic interface. Two ways this gets handled here:

- `LedController` receives the generic `IMeadowDevice` and casts internally: `if (device is F7FeatherV1 f7Device) { ... f7Device.Pins.OnboardLedRed ... }`.
- `RelayController` avoids the cast entirely by not needing named pins inside the controller at all — `MeadowApp.Initialize()` (which has the strongly-typed `Device: F7FeatherV1` from `App<F7FeatherV1>`) resolves `Device.Pins.D05`/`Device.Pins.D06` itself and passes them in as generic `IPin` arguments to `RelayController.Initialize(IPin leftPin, IPin rightPin)`.

Prefer the second pattern (resolve concrete pins at the composition root, inject `IPin`) when practical — it keeps the controller decoupled from the concrete device type. Only reach for the `is F7FeatherV1` cast when the API you need (like named onboard LED pins) genuinely isn't available any other way.

## Watchdog

`WatchdogService : BaseService, IWatchdogService` wraps `device.WatchdogEnable(...)`/`WatchdogReset()`. `Enable(seconds)` arms it; `Pet(seconds)` spins up an **independent background `Thread`** that resets the watchdog on its own schedule. This independence is intentional: it means a long-blocking request handler (e.g. MP3 playback holding the Maple request thread for up to ~14s under `RequestProcessMode.Serial`) doesn't risk tripping the watchdog — petting isn't coupled to request handling at all. Current values here (15s timeout / 10s pet interval) were copied from `meadow_scarecrow`; retune against real command durations if commands ever run longer.

## Maple HTTP server: `Serial` vs `Parallel`

`MapleService.Run()` builds the `MapleServer` with `RequestProcessMode.Serial` here — deliberately, because `shake` (relay sequencing with delays) and `sound` (MP3 playback with a timed delay) must not run concurrently with each other or themselves. `meadow_scarecrow` uses `RequestProcessMode.Parallel` instead, because its `up`/`down` relay commands are simple, non-overlapping toggles with no meaningful concurrency hazard. **When replicating this pattern in a new project, choose the mode based on whether your command handlers have shared mutable state or hardware that can't tolerate concurrent access** — don't default to one or the other without thinking about it.

## Adding a new controller or service

1. Subclass `BaseController` (hardware-shaped) or `BaseService` (app-concern-shaped). Constructor takes `Logger logger` (pass to `base(logger)`) plus whatever else it needs — `IMeadowDevice`, another already-registered controller/service, `INetworkAdapter`, etc.
2. If it owns real hardware (ports, peripherals) and needs idempotent setup, give it a separate `Initialize(...)` method rather than doing it in the constructor, and call that explicitly from `MeadowApp.Initialize()` after `Services.Create<T>()`.
3. Register it in `MeadowApp.Initialize()` in dependency order: `Services.Create<T>()` (or `Create<TImpl, TInterface>()` if other code should depend on an abstraction).
4. If it owns unmanaged/hardware resources, implement `IDisposable` and add a `Services.ContainsRegisteredType<T>() → Services.Get<T>().Dispose()` guard to `MeadowApp.OnShutdown()`.
5. Don't add a restart-testing/chaos service (see `meadow_scarecrow`'s `HeartbeatService`, which intentionally throws periodically to test recovery) to production wiring without being explicitly asked — it's a debug tool, not a default.
