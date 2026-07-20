using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDirectory.Api.Clients;

namespace TrainingDirectory.Api.Controllers
{
    [ApiController]
    [Route("internal/trainees")]
    [Authorize(Roles = "Admin,Mentor")]
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
            var token = Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new { message = "Authorization token is missing" });
            }

            var trainee = await _client.GetTraineeById(id, token, cancellationToken);

            if (trainee == null)
            {
                return NotFound(new { message = "Trainee not found or access denied" });
            }

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