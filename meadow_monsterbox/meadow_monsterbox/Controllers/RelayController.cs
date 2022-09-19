using System;

using Meadow.Foundation.Relays;

namespace meadow_monsterbox.Controllers
{
    internal class RelayController
    {
        protected bool initialized = false;

        protected Relay relayLeft;
        protected Relay relayRight;

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

            relayLeft = new Relay(MeadowApp.Device.CreateDigitalOutputPort(MeadowApp.Device.Pins.D03));
            relayRight = new Relay(MeadowApp.Device.CreateDigitalOutputPort(MeadowApp.Device.Pins.D04));
            initialized = true;
        }

        public void TurnOffLeft()
        {
            relayLeft.IsOn = false;
            Console.WriteLine("Relay Left Is Off.");
        }

        public void TurnOffRight()
        {
            relayRight.IsOn = false;
            Console.WriteLine("Relay Right Is Off.");
        }

        public void TurnOnLeft()
        {
            relayLeft.IsOn = true;
            Console.WriteLine("Relay Left Is On.");
        }

        public void TurnOnRight()
        {
            relayRight.IsOn = true;
            Console.WriteLine("Relay Right Is On.");
        }
    }
}