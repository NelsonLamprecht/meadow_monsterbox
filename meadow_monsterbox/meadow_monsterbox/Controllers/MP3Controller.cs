using System.Threading.Tasks;

using Meadow;
using Meadow.Foundation.Audio.Mp3;

namespace meadow_monsterbox.Controllers
{
    internal class MP3Controller
    {
        // Yx5300 module is wired to the Feather's COM4 UART
        private const string SerialPortName = "COM4";

        private Yx5300 _mp3Player;
        private bool initialized = false;

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }
            _mp3Player = new Yx5300(MeadowApp.Device, MeadowApp.Device.PlatformOS.GetSerialPortName(SerialPortName));

            _mp3Player.SetVolume(30);

            initialized = true;

            Resolver.Log.Info($"{GetType().Name} is initialized.");
        }

        public async Task PlayFile(byte fileNumber, int lengthOfFileInSeconds)
        {
            Resolver.Log.Info($"Playing file: {fileNumber} for {lengthOfFileInSeconds} seconds.");

            byte byteStepUp = (byte)(fileNumber + 1);
            _mp3Player.Play(byteStepUp);

            await Task.Delay((lengthOfFileInSeconds + 1) * 1000);
        }
    }
}
