using Microsoft.AspNetCore.Mvc;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase{
        
        [HttpGet]
        public IActionResult Get(){
            var response = new{
                status = "Healthy",
                application = "TaskManagement API",
                timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }
    }
}