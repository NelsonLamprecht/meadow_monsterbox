using System;
using System.Threading.Tasks;
using System.Threading;

using Meadow;
using Meadow.Foundation.Leds;

namespace meadow_monsterbox.Controllers
{
    internal class LedController : IDisposable
    {
        RgbPwmLed onBoardRGBLed;

        Task animationTask = null;
        CancellationTokenSource cancellationTokenSource = null;

        bool initialized = false;

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }

            onBoardRGBLed = new RgbPwmLed(
                redPwmPin: MeadowApp.Device.Pins.OnboardLedRed,
                greenPwmPin: MeadowApp.Device.Pins.OnboardLedGreen,
                bluePwmPin: MeadowApp.Device.Pins.OnboardLedBlue);
            onBoardRGBLed.SetColor(Color.Red);

            initialized = true;

            Resolver.Log.Info($"{GetType().Name} is initialized.");
        }

        void Stop()
        {
            onBoardRGBLed.StopAnimation();
            cancellationTokenSource?.Cancel();
        }

        public void SetColor(Color color)
        {
            Stop();
            onBoardRGBLed.SetColor(color);
        }

        public void TurnOn()
        {
            Stop();
            onBoardRGBLed.SetColor(GetRandomColor());
            onBoardRGBLed.IsOn = true;
        }

        public void TurnOff()
        {
            Stop();
            onBoardRGBLed.IsOn = false;
        }

        public void StartBlink()
        {
            Stop();
            onBoardRGBLed.StartBlink(GetRandomColor());
        }

        public void StartPulse()
        {
            Stop();
            onBoardRGBLed.StartPulse(GetRandomColor());
        }

        public void StartRunningColors()
        {
            onBoardRGBLed.StopAnimation();

            animationTask = new Task(async () =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                await StartRunningColors(cancellationTokenSource.Token);
            });
            animationTask.Start();
        }

        protected async Task StartRunningColors(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                onBoardRGBLed.SetColor(GetRandomColor());
                await Task.Delay(1000);
            }
        }

        protected Color GetRandomColor()
        {
            var random = new Random();
            return Color.FromHsba((float)random.NextDouble(), 1, 1);
        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
            onBoardRGBLed?.Dispose();
        }
    }
}
