using System;
using System.Threading.Tasks;

using Meadow.Foundation.Audio.Mp3;

namespace meadow_monsterbox.Controllers
{
    internal class MP3Controller : InitalizedBaseController
    {        
        private Yx5300 _mp3Player;

        public static MP3Controller Current { get; private set; }

        private MP3Controller() { }

        static MP3Controller()
        {
            Current = new MP3Controller();
        }

        public override void Initialize()
        {
            if (initialized)
            {
                return;
            }
            _mp3Player = new Yx5300(MeadowApp.Device, MeadowApp.Device.SerialPortNames.Com4);

            _mp3Player.SetVolume(30);

            initialized = true;      
            
            base.Initialize();
        }


        public async Task PlayFile(byte fileNumber, int lengthOfFileInSeconds)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Playing file: {fileNumber} for {lengthOfFileInSeconds} seconds.");

            Byte byteStepUp = (byte)(fileNumber + 1);
            _mp3Player.Play(byteStepUp);

            await Task.Delay((lengthOfFileInSeconds + 1) * 1000);
        }
    }
}
