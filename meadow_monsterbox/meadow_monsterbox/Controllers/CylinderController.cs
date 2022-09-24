using Meadow.Foundation.Relays;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace meadow_monsterbox.Controllers
{
    public class CylinderController
    {
        private readonly Random _random;
        private const int DELAY = 75;

        public CylinderController(Random random)
        {
            _random = random;
        }

        public async Task ShakeAsync()
        {
            Stop();
            Console.WriteLine("Shake.");
            var iterations = _random.Next(25, 51);
            for (int i = 0; i < iterations ; i++)
            {
                Action();
                await Task.Delay(_random.Next(25, 50));
            }
            Stop();
        }

        private void Action()
        {
            Console.WriteLine(Environment.NewLine);

            // either turn it on or turn it off
            var randomNumber = _random.Next(0, 2);
            var randomLeftOrRight = _random.Next(0, 2);

            if (randomNumber == 0)
            {
                if (randomLeftOrRight == 0)
                {
                    RelayController.Current.TurnOffLeft();
                }
                if (randomLeftOrRight == 1)
                {
                    RelayController.Current.TurnOffRight();
                }
            }
            else if (randomNumber == 1)
            {
                if (randomLeftOrRight == 0)
                {
                    RelayController.Current.TurnOnLeft();
                }
                if (randomLeftOrRight == 1)
                {
                    RelayController.Current.TurnOnRight();
                }
            }
        }

        private void Stop()
        {
            Console.WriteLine("Stop.");

            RelayController.Current.TurnOffLeft();

            RelayController.Current.TurnOffRight();

        }
    }   

}
