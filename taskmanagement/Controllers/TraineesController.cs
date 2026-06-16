using Microsoft.AspNetCore.Mvc;
using taskmanagement.Models.DTOs.Trainee;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Models.Enums;
using taskmanagement.Services;
using System.Security.Claims;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineesController : ControllerBase
    {
        private readonly ITraineeService _service;
        private readonly ILogger<TraineesController> _logger;

        public TraineesController(ITraineeService service, ILogger<TraineesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] TraineeStatus? status, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Invalid pagination params PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);

                return BadRequest("pageNumber and pageSize must be greater than 0");
            }

            var result = await _service.GetAll(search, status, pageNumber, pageSize);

            var mapped = result.Data.Select(t => _service.MapToDto(t)).ToList();

            _logger.LogInformation("Trainees fetched Page={Page}, Size={Size}, Count={Count}", pageNumber, pageSize, mapped.Count);

            return Ok(new
            {
                pageNumber = result.PageNumber,
                pageSize = result.PageSize,
                totalRecords = result.TotalRecords,
                data = mapped
            });
        }

        [Authorize(Roles = "Admin]")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddTraineeDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid trainee creation request");
                return BadRequest(ModelState);
            }

            var newTrainee = await _service.Create(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = newTrainee.Id },
                _service.MapToDto(newTrainee));
        }


        [Authorize(Roles = "Admin,Mentor,Trainee")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var trainee = await _service.GetById(id, userId, role!);

            if (trainee == null)
                return NotFound(new { message = "Trainee not found" });

            return Ok(_service.MapToDto(trainee));
        }


        [Authorize(Roles = "Admin,Trainee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] UpdateTraineeDto dto)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var trainee = await _service.Update(id, dto, userId, role!);

            if (trainee == null)
                return NotFound(new { message = "Trainee not found" });

            return Ok(_service.MapToDto(trainee));
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var deleted = await _service.Delete(id, userId, role!);

            if (!deleted)
                return NotFound(new { message = "Trainee not found" });

            return NoContent();
        }
    }
}