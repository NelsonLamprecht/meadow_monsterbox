using System;
using System.Threading.Tasks;

using Meadow;
using Meadow.Foundation.Audio.Mp3;
using Meadow.Logging;
using Wiese.Shared.Controllers;

namespace meadow_monsterbox.Controllers
{
    internal class MP3Controller : BaseController
    {
        private const string serialPortName = "COM4";
        private readonly IMeadowDevice device;
        private bool initialized = false;
        private Yx5300 _mp3Player;
        
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

            _mp3Player = new Yx5300(device, device.PlatformOS.GetSerialPortName(serialPortName));
            _mp3Player.SetVolume(30);
            initialized = true;
        }

        public async Task PlayFile(byte fileNumber, int lengthOfFileInSeconds)
        {
            Logger.Debug(Environment.NewLine);
            Logger.Debug($"Playing file: {fileNumber} for {lengthOfFileInSeconds} seconds.");

            byte byteStepUp = (byte)(fileNumber + 1);
            _mp3Player.Play(byteStepUp);

            await Task.Delay((lengthOfFileInSeconds + 1) * 1000);
        }
    }
}
