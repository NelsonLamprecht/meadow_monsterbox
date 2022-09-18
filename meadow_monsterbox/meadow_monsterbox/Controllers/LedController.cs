using System;
using System.Threading.Tasks;
using System.Threading;

using Meadow.Foundation.Leds;
using Meadow.Foundation;

namespace meadow_monsterbox.Controllers
{
    public class LedController
    {
        RgbPwmLed onBoardRGBLed;

        Task animationTask = null;
        CancellationTokenSource cancellationTokenSource = null;

        protected bool initialized = false;

        public static LedController Current { get; private set; }

        private LedController() { }

        static LedController()
        {
            Current = new LedController();
        }

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }

            onBoardRGBLed = new RgbPwmLed(
                device: MeadowApp.Device,
                redPwmPin: MeadowApp.Device.Pins.OnboardLedRed,
                greenPwmPin: MeadowApp.Device.Pins.OnboardLedGreen,
                bluePwmPin: MeadowApp.Device.Pins.OnboardLedBlue);
            onBoardRGBLed.SetColor(Color.Red);

            initialized = true;
        }

        void Stop()
        {
            onBoardRGBLed.Stop();
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
            onBoardRGBLed.Stop();

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
            return Color.FromHsba(random.NextDouble(), 1, 1);
        }
    }
}
