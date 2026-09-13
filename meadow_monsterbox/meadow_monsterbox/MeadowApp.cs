using System;
using System.Threading.Tasks;

using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;

using meadow_monsterbox.Controllers;
using meadow_monsterbox.Services.DiagnosticsService;
using meadow_monsterbox.Services.MapleService;
using meadow_monsterbox.Services.NetworkService;
using meadow_monsterbox.Services.Watchdog;

namespace meadow_monsterbox
{
    public class MeadowApp : MeadowBase
    {
        private bool _servicesStarted = false;

        public override async Task Initialize()
        {
            Logger.Info("Initializing hardware...");
            Device.Information.DeviceName = "Monsterbox-v1";

            var wifiAdapter = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            // this device always runs on the external antenna; persisted so it's already
            // in effect before AutomaticallyStartNetwork connects on every boot after the first
            wifiAdapter.SetAntenna(AntennaType.External, true);
            Services.Add(wifiAdapter);

            var ledController = Services.Create<LedController>();
            ledController.Initialize();

            var relayController = Services.Create<RelayController>();
            relayController.Initialize(Device.Pins.D05, Device.Pins.D06);

            var mp3Controller = Services.Create<MP3Controller>();
            mp3Controller.Initialize();

            Services.Create<DiagnosticsService>();
            Services.Create<WatchdogService, IWatchdogService>();
            Services.Create<NetworkService>();
            Services.Create<CylindersController>();
            Services.Create<MapleService>();

            wifiAdapter.NetworkConnected += OnWifiConnected;
            wifiAdapter.NetworkDisconnected += (sender, args) =>
                Logger.Warn("WiFi disconnected.");

            await base.Initialize();
        }

        public override Task Run()
        {
            var watchdog = Services.Get<IWatchdogService>();
            watchdog.Enable(15);
            watchdog.Pet(10);

            // network connection is handled by Meadow OS via wifi.config.yaml and
            // AutomaticallyStartNetwork in meadow.config.yaml; see OnWifiConnected
            Logger.Info("Running. Waiting for WiFi connection...");
            return base.Run();
        }

        private void OnWifiConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Logger.Info($"WiFi connected. IP: {args.IpAddress}");

            var networkService = Services.Get<NetworkService>();
            networkService.NetworkIsConnected(sender);

            // AutomaticallyReconnect can raise this again after a drop; only start the Maple server once
            if (_servicesStarted)
            {
                return;
            }
            _servicesStarted = true;

            Services.Get<MapleService>().Run();

            Services.Get<LedController>().SetColor(Color.Green);
        }

        public override Task OnError(Exception e)
        {
            Logger.Error($"Unhandled application error: {e.Message}");
            return base.OnError(e);
        }

        public override Task OnShutdown()
        {
            Logger.Info("Shutting down...");

            if (Services.ContainsRegisteredType<LedController>())
            {
                Services.Get<LedController>().Dispose();
            }

            if (Services.ContainsRegisteredType<RelayController>())
            {
                Services.Get<RelayController>().Dispose();
            }

            return base.OnShutdown();
        }
    }
}
