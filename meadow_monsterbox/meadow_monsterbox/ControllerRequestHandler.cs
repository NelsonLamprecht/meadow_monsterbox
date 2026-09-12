using System;
using System.Threading.Tasks;

using Meadow;
using Meadow.Foundation.Web.Maple.Routing;
using Meadow.Foundation.Web.Maple;

using meadow_monsterbox.Controllers;

namespace meadow_monsterbox
{
    public class ControllerRequestHandler : RequestHandlerBase
    {
        public ControllerRequestHandler() { }

        public override bool IsReusable => true;

        [HttpPost("/sound")]
        public async Task<IActionResult> SoundAsync()
        {
            try
            {
                var fileNumber = Convert.ToByte(QueryString["filenumber"]);
                var fileDuration = Convert.ToInt32(QueryString["fileduration"]);
                await Resolver.Services.Get<MP3Controller>().PlayFile(fileNumber, fileDuration);
            }
            catch (Exception ex)
            {
                Resolver.Log.Error(ex.Message);
            }
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

            await Resolver.Services.Get<CylindersController>().ShakeAsync(config);
            return new OkResult();
        }
    }
}
