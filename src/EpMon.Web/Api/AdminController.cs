using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EpMon.Web.Api
{
    public class AdminController : BaseApiController
    {
        private IHostApplicationLifetime ApplicationLifetime { get; set; }
        private readonly ILogger _logger;

        public AdminController(ILogger<AdminController> logger, IHostApplicationLifetime appLifetime)
        {
            ApplicationLifetime = appLifetime;
            _logger = logger;
        }

        [HttpPost("/api/shutdown")]
        public ActionResult Shutdown()
        {
            _logger.LogInformation("Stopping Application...");

            ApplicationLifetime.StopApplication();

            return Ok();
        }
    }
}