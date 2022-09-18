using Meadow.Foundation.Web.Maple.Server.Routing;
using Meadow.Foundation.Web.Maple.Server;
using meadow_monsterbox.Controllers;

namespace meadow_monsterbox
{
    public class LedControllerRequestHandler : RequestHandlerBase
    {
        public LedControllerRequestHandler() { }

        [HttpPost("/turnon")]
        public IActionResult TurnOn()
        {
            LedController.Current.TurnOn();

            RelayController.Current.TurnOnLeft();
            RelayController.Current.TurnOnRight();

            return new OkResult();
        }

        [HttpPost("/turnoff")]
        public IActionResult TurnOff()
        {
            LedController.Current.TurnOff();

            RelayController.Current.TurnOffLeft();
            RelayController.Current.TurnOffRight();

            return new OkResult();
        }

        [HttpPost("/startblink")]
        public IActionResult StartBlink()
        {
            LedController.Current.StartBlink();
            return new OkResult();
        }

        [HttpPost("/startpulse")]
        public IActionResult StartPulse()
        {
            LedController.Current.StartPulse();
            return new OkResult();
        }

        [HttpPost("/startrunningcolors")]
        public IActionResult StartRunningColors()
        {
            LedController.Current.StartRunningColors();
            return new OkResult();
        }
    }
}
