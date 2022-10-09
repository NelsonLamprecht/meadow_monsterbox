using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Meadow.Foundation.Web.Maple.Server.Routing;
using Meadow.Foundation.Web.Maple.Server;

using meadow_monsterbox.Controllers;

namespace meadow_monsterbox
{
    public class ControllerRequestHandler : RequestHandlerBase
    {
        public ControllerRequestHandler() { }

        public override bool IsReusable => true;

        //[HttpPost("/turnon")]
        //public IActionResult TurnOn()
        //{
        //    LedController.Current.TurnOn();
        //    return new OkResult();
        //}

        //[HttpPost("/turnoff")]
        //public IActionResult TurnOff()
        //{
        //    LedController.Current.TurnOff();
        //    return new OkResult();
        //}


        [HttpPost("/sound")]
        public async Task<IActionResult> SoundAsync()
        {
            try
            {
                var fileNumber = Convert.ToByte(QueryString["filenumber"]);
                var fileDuration = Convert.ToInt32(QueryString["fileduration"]);
                await MP3Controller.Current.PlayFile(fileNumber, fileDuration);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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

            await MeadowApp.Current.Cylinders.ShakeAsync(config);
            return new OkResult();
        }
    }
}
