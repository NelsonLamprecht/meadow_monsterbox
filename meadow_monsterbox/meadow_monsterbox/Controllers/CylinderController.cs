using System;
using System.Threading.Tasks;

using Meadow;
using Meadow.Logging;

namespace meadow_monsterbox.Controllers
{
    internal class CylindersController : BaseController
    {
        private readonly RelayController relayController;
        private readonly Random _random;

        public CylindersController(Logger logger, RelayController relayController) : base(logger)
        {
            this.relayController = relayController;
            _random = new Random();
        }

        public async Task ShakeAsync(ShakeConfiguration config)
        {
            Stop();
            Logger.Info($"Shake. Iterations: {config.GetIterations()}");

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
                    relayController.TurnOffLeft();
                    await Task.Delay(config.GetDelay());
                }
                else if (randomLeftOrRight == 1)
                {
                    relayController.TurnOffRight();
                    await Task.Delay(config.GetDelay());
                }
            }
            else if (randomNumber == 1)
            {
                if (randomLeftOrRight == 0)
                {
                    relayController.TurnOnLeft();
                    await Task.Delay(config.GetDelay());
                }
                else if (randomLeftOrRight == 1)
                {
                    relayController.TurnOnRight();
                    await Task.Delay(config.GetDelay());
                }
            }
        }

        private void Stop()
        {
            Logger.Info("Stop.");
            relayController.TurnOffLeft();
            relayController.TurnOffRight();
        }
    }
}
