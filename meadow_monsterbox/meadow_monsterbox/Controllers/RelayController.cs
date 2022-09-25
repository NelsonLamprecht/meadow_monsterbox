using System;

using Meadow.Foundation.Relays;

namespace meadow_monsterbox.Controllers
{
    internal class RelayController
    {
        protected bool initialized = false;

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

        private RelayController()
        {
        }

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }

            relayLeft = new Relay(MeadowApp.Device.CreateDigitalOutputPort(MeadowApp.Device.Pins.D03,false,Meadow.Hardware.OutputType.OpenDrain),Meadow.Peripherals.Relays.RelayType.NormallyClosed);
            relayRight = new Relay(MeadowApp.Device.CreateDigitalOutputPort(MeadowApp.Device.Pins.D04,false,Meadow.Hardware.OutputType.OpenDrain),Meadow.Peripherals.Relays.RelayType.NormallyClosed);
            initialized = true;
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