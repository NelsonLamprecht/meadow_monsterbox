using System;
using System.Threading.Tasks;

namespace meadow_monsterbox.Controllers
{
    public class CylindersController
    {
        private readonly Random _random;

        public CylindersController()
        {
            _random = new Random();
        }

        public async Task ShakeAsync(ShakeConfiguration config)
        {
            Stop();
            Console.WriteLine("Shake.");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Iterations: {config.GetIterations()}");

            for (int i = 0; i <= config.GetIterations() ; i++)
            {
                await ActionAsync(config);
            }
            Stop();
        }

        private async Task ActionAsync(ShakeConfiguration config)
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
                    await Task.Delay(config.GetDelay());
                }
                else if (randomLeftOrRight == 1)
                {
                    RelayController.Current.TurnOffRight();
                    await Task.Delay(config.GetDelay());
                }                
            }
            else if (randomNumber == 1)
            {
                if (randomLeftOrRight == 0)
                {
                    RelayController.Current.TurnOnLeft();
                    await Task.Delay(config.GetDelay());
                }
                else if (randomLeftOrRight == 1)
                {
                    RelayController.Current.TurnOnRight();
                    await Task.Delay(config.GetDelay());
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
