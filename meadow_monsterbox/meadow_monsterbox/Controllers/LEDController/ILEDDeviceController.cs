using Meadow.Foundation;

namespace meadow_monsterbox.Controllers.LEDController
{
    public interface ILEDDeviceController : IOnOrOffController
    {
        void SetColor(Color color);

        void Stop();

        void StartBlink(Color color);
    }
}
