using System;

using Meadow;
using Meadow.Foundation.Relays;
using Meadow.Hardware;
using Meadow.Peripherals.Relays;

namespace meadow_monsterbox.Controllers
{
    /// <summary>
    /// The commands all inverted since the pneumatics are keeping the values closed
    /// </summary>
    internal class RelayController : IDisposable
    {
        private IDigitalOutputPort _leftRelayPort;
        private IDigitalOutputPort _rightRelayPort;
        private Relay relayLeft;
        private Relay relayRight;
        private bool _debug = false;
        private bool initialized = false;

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }
            // true so port is closed as quickly as possible when board boots up
            _leftRelayPort = MeadowApp.Device.CreateDigitalOutputPort(MeadowApp.Device.Pins.D05, true, OutputType.OpenDrain);
            _rightRelayPort = MeadowApp.Device.CreateDigitalOutputPort(MeadowApp.Device.Pins.D06, true, OutputType.OpenDrain);
            relayLeft = new Relay(_leftRelayPort, RelayType.NormallyOpen);
            relayRight = new Relay(_rightRelayPort, RelayType.NormallyOpen);
            TurnOffLeft();
            TurnOffRight();
            initialized = true;

            Resolver.Log.Info($"{GetType().Name} is initialized.");
        }

        public void DebugOff()
        {
            _debug = false;
        }

        public void DebugOn()
        {
            _debug = true;
        }

        public void TurnOffLeft()
        {
            relayLeft.State = RelayState.Open;
            if (_debug)
            {
                Resolver.Log.Info("Relay Left Is Off.");
            }
        }

        public void TurnOffRight()
        {
            relayRight.State = RelayState.Open;
            if (_debug)
            {
                Resolver.Log.Info("Relay Right Is Off.");
            }
        }

        public void TurnOnLeft()
        {
            relayLeft.State = RelayState.Closed;
            if(_debug)
            {
                Resolver.Log.Info("Relay Left Is On.");
            }
        }

        public void TurnOnRight()
        {
            relayRight.State = RelayState.Closed;
            if (_debug)
            {
                Resolver.Log.Info("Relay Right Is On.");
            }
        }

        public void Dispose()
        {
            _leftRelayPort?.Dispose();
            _rightRelayPort?.Dispose();
        }
    }
}