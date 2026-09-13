using System.Threading.Tasks;

using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Audio.Mp3;
using Meadow.Logging;

namespace meadow_monsterbox.Controllers
{
    internal class MP3Controller : BaseController
    {
        // Yx5300 module is wired to the Feather's COM4 UART
        private const string SerialPortName = "COM4";

        private readonly IMeadowDevice device;

        private Yx5300 _mp3Player;
        private bool initialized = false;

        public MP3Controller(Logger logger, IMeadowDevice device) : base(logger)
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
                _mp3Player = new Yx5300(f7Device, f7Device.PlatformOS.GetSerialPortName(SerialPortName));

                _mp3Player.SetVolume(30);
            }

            initialized = true;

            Logger.Info($"{GetType().Name} is initialized.");
        }

        public async Task PlayFile(byte fileNumber, int lengthOfFileInSeconds)
        {
            Logger.Info($"Playing file: {fileNumber} for {lengthOfFileInSeconds} seconds.");

            byte byteStepUp = (byte)(fileNumber + 1);
            _mp3Player.Play(byteStepUp);

            await Task.Delay((lengthOfFileInSeconds + 1) * 1000);
        }
    }
}
