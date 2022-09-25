using System.Threading.Tasks;

using Meadow.Foundation.Web.Maple.Server.Routing;
using Meadow.Foundation.Web.Maple.Server;

using meadow_monsterbox.Controllers;
using Meadow.Units;

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
            var config = new ShakeConfiguration();

            if (int.TryParse(QueryString["bi"], out var beginIterations))
            {
                config.BeginIterations = beginIterations;
            }

            if (int.TryParse(QueryString["ei"], out var endIterations))
            {
                config.EndIterations = endIterations;
            }

            if (int.TryParse(QueryString["bd"], out var beginDelay))
            {
                config.BeginDelay = beginDelay;
            }

            if (int.TryParse(QueryString["ed"], out var endDelay))
            {
                config.EndDelay = endDelay;
            }

            await MeadowApp.Current.Cylinders.ShakeAsync(config);
            return new OkResult();
        }
    }
}
