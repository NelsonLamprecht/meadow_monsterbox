using System;

using Meadow.Foundation.Relays;
using Meadow.Hardware;

namespace meadow_monsterbox.Controllers
{
    /// <summary>
    /// The commands all inverted since the pneumatics are keeping the values closed
    /// </summary>
    internal class RelayController: InitalizedBaseController
    {
        private Relay relayLeft;
        private Relay relayRight;
        private bool _debug = false;

        public static RelayController Current
        {
            get;
            private set;
        }

        static RelayController()
        {
            Current = new RelayController();
        }

        private RelayController() {  }

        public override void Initialize()
        {
            if (initialized)
            {
                return;
            }
            // true so port is closed as quickly as possible when board boots up
            relayLeft = new Relay(MeadowApp.Device.CreateDigitalOutputPort(MeadowApp.Device.Pins.D03,true,OutputType.OpenDrain),Meadow.Peripherals.Relays.RelayType.NormallyOpen);
            relayRight = new Relay(MeadowApp.Device.CreateDigitalOutputPort(MeadowApp.Device.Pins.D04,true,OutputType.OpenDrain),Meadow.Peripherals.Relays.RelayType.NormallyOpen);
            TurnOffLeft();
            TurnOffRight();
            initialized = true;

            base.Initialize();
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
            relayLeft.IsOn = !false;
            if (_debug)
            {
                Console.WriteLine("Relay Left Is Off.");
            }
        }

        public void TurnOffRight()
        {
            relayRight.IsOn = !false;
            if (_debug)
            {
                Console.WriteLine("Relay Right Is Off.");
            }
        }

        public void TurnOnLeft()
        {
            relayLeft.IsOn = !true;
            if(_debug)
            {
                Console.WriteLine("Relay Left Is On.");
            }
        }

        public void TurnOnRight()
        {
            relayRight.IsOn = !true;
            if (_debug)
            {
                Console.WriteLine("Relay Right Is On.");
            }
        }
    }
}