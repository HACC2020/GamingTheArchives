using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ArchiveSiteBackend.Api.Controllers {
    [ApiController]
    [Route("api/health")]
    public class HealthCheckController : Controller {
        private readonly ILogger<HealthCheckController> logger;

        public HealthCheckController(ILogger<HealthCheckController> logger) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Get() {
            this.logger.LogTrace("Health Check Request Received: {Timestamp}", DateTime.UtcNow);

            return Ok(new {
                Version =
                    typeof(HealthCheckController).Assembly
                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                        ?.InformationalVersion,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
