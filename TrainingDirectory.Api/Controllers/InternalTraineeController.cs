using Microsoft.AspNetCore.Mvc;
using TrainingDirectory.Api.Models;
using TrainingDirectory.Api.Clients;

namespace TrainingDirectory.Api.Controllers
{
    [ApiController]
    [Route("internal/trainees")]
    public class InternalTraineeController : ControllerBase
    {
        private readonly ITaskManagementClient _client;

        public InternalTraineeController(ITaskManagementClient client)
        {
            _client = client;
        }

        [HttpGet("trainee/{id}")]
        public async Task<IActionResult> GetTrainee(int id, CancellationToken cancellationToken)
        {
            var trainee = await _client.GetTraineeById(id,cancellationToken);

            if (trainee == null)
                return NotFound(new { message = "Trainee not found or service unavailable" });

            var result = new
            {
                traineeId = trainee.Id,
                name = $"{trainee.FirstName} {trainee.LastName}",
                email = trainee.Email,
                techStack = trainee.TechStack,
                status = trainee.Status
            };

            return Ok(result);
        }

    }
}