using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("live")]
        public IActionResult Live()
        {
            _logger.LogInformation("Liveness check called");
            return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }

        [AllowAnonymous]
        [HttpGet("ready")]
        public async Task<IActionResult> Ready()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                services = report.Entries.Select(e => new
                {
                    service = e.Key,
                    status = e.Value.Status.ToString()
                })
            };

            _logger.LogInformation("Readiness check executed with status {Status}", report.Status);

            return report.Status == HealthStatus.Healthy ? Ok(response) : StatusCode(503, response);
        }
    }
}