using System;

using Meadow;
using Meadow.Foundation.Relays;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Relays;

namespace meadow_monsterbox.Controllers
{
    /// <summary>
    /// The commands all inverted since the pneumatics are keeping the values closed
    /// </summary>
    internal class RelayController : BaseController, IDisposable
    {
        private readonly IMeadowDevice device;

        private IDigitalOutputPort _leftRelayPort;
        private IDigitalOutputPort _rightRelayPort;
        private Relay relayLeft;
        private Relay relayRight;
        private bool _debug = false;
        private bool initialized = false;

        public RelayController(Logger logger, IMeadowDevice device) : base(logger)
        {
            this.device = device;
        }

        public void Initialize(IPin leftPin, IPin rightPin)
        {
            if (initialized)
            {
                return;
            }
            // true so port is closed as quickly as possible when board boots up
            _leftRelayPort = device.CreateDigitalOutputPort(leftPin, true, OutputType.OpenDrain);
            _rightRelayPort = device.CreateDigitalOutputPort(rightPin, true, OutputType.OpenDrain);
            relayLeft = new Relay(_leftRelayPort, RelayType.NormallyOpen);
            relayRight = new Relay(_rightRelayPort, RelayType.NormallyOpen);
            TurnOffLeft();
            TurnOffRight();
            initialized = true;

            Logger.Info($"{GetType().Name} is initialized.");
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
                Logger.Info("Relay Left Is Off.");
            }
        }

        public void TurnOffRight()
        {
            relayRight.State = RelayState.Open;
            if (_debug)
            {
                Logger.Info("Relay Right Is Off.");
            }
        }

        public void TurnOnLeft()
        {
            relayLeft.State = RelayState.Closed;
            if(_debug)
            {
                Logger.Info("Relay Left Is On.");
            }
        }

        public void TurnOnRight()
        {
            relayRight.State = RelayState.Closed;
            if (_debug)
            {
                Logger.Info("Relay Right Is On.");
            }
        }

        public void Dispose()
        {
            _leftRelayPort?.Dispose();
            _rightRelayPort?.Dispose();
        }
    }
}
