using System;
using System.Threading.Tasks;

using meadow_monsterbox.Controllers;
using Meadow.Logging;

namespace meadow_monsterbox.Controllers
{
    public class PairedRelayController
    {
        private readonly Random _random;
        private readonly RelayController leftRelayController;
        private readonly RelayController rightRelayController;

        public PairedRelayController(Logger logger, RelayController leftRelayController, RelayController rightRelayController)
        {
            Logger = logger;
            this.leftRelayController = leftRelayController;
            this.rightRelayController = rightRelayController;
            _random = new Random();
        }

        public Logger Logger { get; }

        public async Task ShakeAsync(ShakeConfiguration config)
        {
            Stop();
            Logger.Debug("Shake.");
            Logger.Info($"Iterations: {config.GetIterations()}");

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
                    leftRelayController.TurnOff();
                    await Task.Delay(config.GetDelay());
                }
                else if (randomLeftOrRight == 1)
                {
                    rightRelayController.TurnOff();
                    await Task.Delay(config.GetDelay());
                }
            }
            else if (randomNumber == 1)
            {
                if (randomLeftOrRight == 0)
                {
                    leftRelayController.TurnOn();
                    await Task.Delay(config.GetDelay());
                }
                else if (randomLeftOrRight == 1)
                {
                    rightRelayController.TurnOn();
                    await Task.Delay(config.GetDelay());
                }
            }
        }

        private void Stop()
        {
            Logger.Info("Stop.");
            leftRelayController.TurnOff();
            rightRelayController.TurnOff();
        }
    }   
}
