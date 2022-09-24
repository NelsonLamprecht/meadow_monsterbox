using System.Threading.Tasks;

using Meadow.Foundation.Web.Maple.Server.Routing;
using Meadow.Foundation.Web.Maple.Server;

using meadow_monsterbox.Controllers;

namespace meadow_monsterbox
{
    public class ControllerRequestHandler : RequestHandlerBase
    {
        public ControllerRequestHandler() { }

        [HttpPost("/turnon")]
        public IActionResult TurnOn()
        {
            LedController.Current.TurnOn();
            return new OkResult();
        }

        [HttpPost("/turnoff")]
        public IActionResult TurnOff()
        {
            LedController.Current.TurnOff();
            return new OkResult();
        }       

        [HttpPost("/shake")]
        public async Task<IActionResult> ShakeAsync()
        {            
            await MeadowApp.Current.BoxCylinders.ShakeAsync();
            return new OkResult();
        }
    }
}
