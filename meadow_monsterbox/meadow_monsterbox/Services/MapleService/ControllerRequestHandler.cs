using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Meadow;
using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using Meadow.Logging;

using meadow_monsterbox.Controllers;

namespace meadow_monsterbox.Services.MapleService
{
    public class ControllerRequestHandler : RequestHandlerBase
    {
        public ControllerRequestHandler()
        {
        }

        public override bool IsReusable => true;

        private Logger Logger
        {
            get
            {
                return Resolver.Log;
            }
        }

        private PairedRelayController PairedRelayController
        {
            get
            {
                return Resolver.Services.Get<PairedRelayController>();
            }
        }

        private MP3Controller MP3Controller
        {
            get
            {
                return Resolver.Services.Get<MP3Controller>();
            }
        }


        [HttpPost("/sound")]
        public async Task<IActionResult> SoundAsync()
        {
            try
            {
                var fileNumber = Convert.ToByte(QueryString["filenumber"]);
                var fileDuration = Convert.ToInt32(QueryString["fileduration"]);
                await MP3Controller.PlayFile(fileNumber, fileDuration);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
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

            await PairedRelayController.ShakeAsync(config);
            return new OkResult();
        }
    }
}
