using System;

using Meadow;
using Meadow.Foundation.Web.Maple.Routing;
using Meadow.Foundation.Web.Maple;

using meadow_monsterbox.Controllers;

namespace meadow_monsterbox
{
    public class ControllerRequestHandler : RequestHandlerBase
    {
        private readonly MP3Controller _mp3Controller;
        private readonly CylindersController _cylindersController;

        // Maple constructs this handler itself (outside the DI container), so resolve
        // dependencies from the global registry once here rather than per request.
        // Safe because MapleService only starts after MeadowApp.Initialize() has
        // registered these controllers.
        public ControllerRequestHandler()
        {
            _mp3Controller = Resolver.Services.Get<MP3Controller>();
            _cylindersController = Resolver.Services.Get<CylindersController>();
        }

        public override bool IsReusable => true;

        [HttpPost("/sound")]
        public IActionResult Sound()
        {
            try
            {
                var fileNumber = Convert.ToByte(QueryString["filenumber"]);
                _mp3Controller.PlayFile(fileNumber);
            }
            catch (Exception ex)
            {
                Resolver.Log.Error(ex.Message);
            }
            return new OkResult();
        }

        [HttpPost("/shake")]
        public IActionResult Shake()
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

            _cylindersController.TryShake(config);
            return new OkResult();
        }
    }
}
