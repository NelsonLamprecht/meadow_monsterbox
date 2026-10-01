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

        // 0 = idle, 1 = shaking; set on the request thread and cleared from the
        // background shake continuation, so access it via Interlocked
        private int _isShaking;

        // Starts a shake and returns immediately so the HTTP request isn't held open
        // for the whole sequence (and a /sound request right after isn't queued behind
        // it). Returns false (and ignores the request) if a shake is already running.
        public bool TryShake(ShakeConfiguration config)
        {
            if (Interlocked.CompareExchange(ref _isShaking, 1, 0) != 0)
            {
                Logger.Warn("Already shaking; ignoring request.");
                return false;
            }

            _ = ShakeInBackground(config);
            return true;
        }

        private async Task ShakeInBackground(ShakeConfiguration config)
        {
            try
            {
                await ShakeAsync(config);
            }
            catch (Exception ex)
            {
                Logger.Error($"Shake failed: {ex.Message}");
                Stop();
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
