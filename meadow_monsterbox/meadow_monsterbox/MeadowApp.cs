using System;
using System.Threading.Tasks;

using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using meadow_monsterbox.Controllers;

namespace meadow_monsterbox
{
    public class MeadowApp : App<F7FeatherV1>
    {
        private MapleServer _mapleServer;
        private bool _servicesStarted = false;

        public override async Task Initialize()
        {
            Resolver.Log.Info("Initializing hardware...");
            Device.Information.DeviceName = "Monsterbox-v1";

            var ledController = new LedController();
            ledController.Initialize();
            Resolver.Services.Add(ledController);

            var relayController = new RelayController();
            relayController.Initialize();
            Resolver.Services.Add(relayController);

            var mp3Controller = new MP3Controller();
            mp3Controller.Initialize();
            Resolver.Services.Add(mp3Controller);

            var wifiAdapter = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            // this device always runs on the external antenna; persisted so it's already
            // in effect before AutomaticallyStartNetwork connects on every boot after the first
            wifiAdapter.SetAntenna(AntennaType.External, true);

            wifiAdapter.NetworkConnected += OnWifiConnected;
            wifiAdapter.NetworkDisconnected += (sender, args) =>
                Resolver.Log.Warn("WiFi disconnected.");

            await base.Initialize();
        }

        public override Task Run()
        {
            // network connection is handled by Meadow OS via wifi.config.yaml and
            // AutomaticallyStartNetwork in meadow.config.yaml; see OnWifiConnected
            Resolver.Log.Info("Running. Waiting for WiFi connection...");
            return base.Run();
        }

        private void OnWifiConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Resolver.Log.Info($"WiFi connected. IP: {args.IpAddress}");

            // AutomaticallyReconnect can raise this again after a drop; only start services once
            if (_servicesStarted)
            {
                return;
            }
            _servicesStarted = true;

            _mapleServer = new MapleServer(args.IpAddress, 5417, true, RequestProcessMode.Serial, null)
            {
                AdvertiseIntervalMs = 1500, // every 1.5 seconds
                DeviceName = Device.Information.DeviceName
            };
            _mapleServer.Start();

            Resolver.Services.Add(new CylindersController());

            Resolver.Services.Get<LedController>().SetColor(Color.Green);
        }

        public override Task OnError(Exception e)
        {
            Resolver.Log.Error($"Unhandled application error: {e.Message}");
            return base.OnError(e);
        }

        public override Task OnShutdown()
        {
            Resolver.Log.Info("Shutting down...");

            if (Resolver.Services.ContainsRegisteredType<LedController>())
            {
                Resolver.Services.Get<LedController>().Dispose();
            }

            if (Resolver.Services.ContainsRegisteredType<RelayController>())
            {
                Resolver.Services.Get<RelayController>().Dispose();
            }

            return base.OnShutdown();
        }
    }
}
