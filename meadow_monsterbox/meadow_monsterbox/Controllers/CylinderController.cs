using System;
using System.Threading;
using System.Threading.Tasks;

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

        // 0 = idle, 1 = shaking; Maple runs requests in parallel, so two /shake
        // requests can race for this — access it via Interlocked
        private int _isShaking;

        // Runs the whole shake and completes when it's done, so the /shake response
        // tells the caller the shake has finished. Returns false immediately (without
        // shaking) if another shake is already running. Exceptions propagate to the
        // caller after the relays are switched off.
        public async Task<bool> TryShakeAsync(ShakeConfiguration config)
        {
            if (Interlocked.CompareExchange(ref _isShaking, 1, 0) != 0)
            {
                Logger.Warn("Already shaking; ignoring request.");
                return false;
            }

            try
            {
                await ShakeAsync(config);
                return true;
            }
            catch
            {
                // never leave a cylinder stuck on
                Stop();
                throw;
            }
            finally
            {
                Interlocked.Exchange(ref _isShaking, 0);
            }
        }

        private async Task ShakeAsync(ShakeConfiguration config)
        {
            Stop();
            var iterations = config.GetIterations();
            Logger.Info($"Shake. Iterations: {iterations}");

            for (int i = 0; i <= iterations; i++)
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
