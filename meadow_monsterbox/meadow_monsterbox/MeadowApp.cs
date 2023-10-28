using System;
using System.Threading.Tasks;

using Meadow.Foundation;
using Meadow.Hardware;

using Wiese.Shared;

using meadow_monsterbox.Controllers.LEDController;
using meadow_monsterbox.Services.DiagnosticsService;
using meadow_monsterbox.Services.MapleService;
using meadow_monsterbox.Services.NetworkService;
using meadow_monsterbox.Services.Watchdog;

using meadow_monsterbox.Controllers;

namespace meadow_monsterbox
{
    public class MeadowApp : MeadowBase
    {
        private ILEDDeviceController _ledDevice;

        public override async Task Initialize()
        {
            var network = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            network.SetAntenna(AntennaType.External);
            network.NetworkConnected += NetworkConnected;

            Services.Add(network);

            //rework the device dependancy for the controller so its not just the meadow device
            _ledDevice = Services.Create<OnBoardLEDDeviceController, ILEDDeviceController>();

            var leftRelayController = new RelayController(this.Logger,this.MeadowDevice);
            leftRelayController.Initialize(Device.Pins.D05);
            var rightRelayController = new RelayController(this.Logger, this.MeadowDevice);
            rightRelayController.Initialize(Device.Pins.D06);

            var pairedRelayController = new PairedRelayController(this.Logger, leftRelayController, rightRelayController);
            Services.Add(pairedRelayController);

            var mp3Controller = Services.Create<MP3Controller>();
            mp3Controller.Initialize();

            Services.Create<WatchdogService, IWatchdogService>();
            Services.Create<DiagnosticsService>();
            Services.Create<NetworkService>();
            Services.Create<MapleService>();

            await base.Initialize();
        }

        public override async Task Run()
        {
            try
            {
                Logger.Debug($"+Run");

                var diagnostics = Services.Get<DiagnosticsService>();
                diagnostics.OutputDeviceInfo();
                diagnostics.OutputMeadowOSInfo();
                diagnostics.OutputNtpInfo();

                var w = Services.Get<IWatchdogService>();
                w.Enable(15);
                w.Pet(10);

                var relayController = Services.Get<RelayController>();
                await relayController.Run();

                _ledDevice.StartBlink(Color.Blue);
            }

            catch (Exception ex)
            {
                _ledDevice.SetColor(Color.Red);
                Logger.Error(ex.ToString());
            }
            await base.Run();
        }

        private void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            var networkService = Services.Get<NetworkService>();
            networkService.NetworkIsConnected(sender);

            var mapleService = Services.Get<MapleService>();
            mapleService.Run();

            var ledDeviceController = Services.Get<ILEDDeviceController>();
            ledDeviceController.StartBlink(Color.Green);
        }
    }
}