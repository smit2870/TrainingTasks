using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = new
                {
                    status = "Healthy",
                    application = "TaskManagement API",
                    timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("Health check requested at {Timestamp}", response.timestamp);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(500, "Service unhealthy");
            }
        }
    }
}
