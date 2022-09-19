using Meadow.Foundation.Relays;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace meadow_monsterbox.Controllers
{
    public class CylinderController
    {
        private readonly Cylinders _cylinder;
        private readonly Random _random;        

        public CylinderController(Cylinders cylinder, Random random)
        {
            _cylinder = cylinder;
            _random = random;            
        }

        public async Task ShakeAsync()
        {            
            for (int i = 0; i < 10; i++)
            {
                await Action();
            }            
        }

        private async Task Action()
        {
            // either turn it on or turn it off
            var randomNumber = _random.Next(0, 2);
            if (randomNumber == 0)
            {
                if (_cylinder == Cylinders.Left)
                {
                    RelayController.Current.TurnOffLeft();
                }
                else if (_cylinder == Cylinders.Right)
                {
                    RelayController.Current.TurnOffRight();
                }
            }
            else if (randomNumber == 1)
            {
                if (_cylinder == Cylinders.Left)
                {
                    RelayController.Current.TurnOnLeft();
                }
                else if (_cylinder == Cylinders.Right)
                {
                    RelayController.Current.TurnOnRight();
                }
            }

            await Task.Delay(1);
        }

        private void Stop()
        {
            Console.WriteLine("Stop.");
            RelayController.Current.TurnOffLeft();
            RelayController.Current.TurnOffRight();
        }
    }

    public enum Cylinders
    {
        Left = 1,
        Right = 2
    }
}
