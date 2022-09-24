using System;
using System.Threading.Tasks;

namespace meadow_monsterbox.Controllers
{
    public class CylinderController
    {
        private readonly Random _random;

        public CylinderController(Random random)
        {
            _random = random;
        }

        public async Task ShakeAsync()
        {
            Stop();
            Console.WriteLine("Shake.");
            var iterations = _random.Next(25, 51);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Iterations: {iterations}");

            for (int i = 0; i < iterations ; i++)
            {
                await ActionAsync();
            }
            Stop();
        }

        private async Task ActionAsync()
        {
            // either turn it on or turn it off
            var randomNumber = _random.Next(0, 2);

            // left or right
            var randomLeftOrRight = _random.Next(0, 2);

            if (randomNumber == 0)
            {
                if (randomLeftOrRight == 0)
                {
                    RelayController.Current.TurnOffLeft();
                }
                else if (randomLeftOrRight == 1)
                {
                    RelayController.Current.TurnOffRight();                   
                }
                await Task.Delay(_random.Next(25, 50));
            }
            else if (randomNumber == 1)
            {
                if (randomLeftOrRight == 0)
                {
                    RelayController.Current.TurnOnLeft();
                }
                else if (randomLeftOrRight == 1)
                {
                    RelayController.Current.TurnOnRight();
                }
                await Task.Delay(_random.Next(25, 50));
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
