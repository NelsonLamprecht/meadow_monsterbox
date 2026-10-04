using System;
using System.Threading.Tasks;
using System.Threading;

using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Leds;
using Meadow.Logging;

namespace meadow_monsterbox.Controllers
{
    internal class LedController : BaseController, IDisposable
    {
        private readonly IMeadowDevice device;

        RgbPwmLed onBoardRGBLed;

        Task animationTask = null;
        CancellationTokenSource cancellationTokenSource = null;

        bool initialized = false;

        public LedController(Logger logger, IMeadowDevice device) : base(logger)
        {
            this.device = device;
        }

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }

            if (device is F7FeatherV1 f7Device)
            {
                onBoardRGBLed = new RgbPwmLed(
                    redPwmPin: f7Device.Pins.OnboardLedRed,
                    greenPwmPin: f7Device.Pins.OnboardLedGreen,
                    bluePwmPin: f7Device.Pins.OnboardLedBlue);
                onBoardRGBLed.SetColor(Color.Red);
            }

            initialized = true;

            Logger.Info($"{GetType().Name} is initialized.");
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
