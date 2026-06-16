using Microsoft.AspNetCore.Mvc;
using taskmanagement.Models.DTOs.Trainee;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Models.Enums;
using taskmanagement.Services;

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

        [Authorize(Roles = "Admin,Mentor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trainee = await _service.GetById(id);

            if (trainee == null)
            {
                _logger.LogWarning("Trainee not found Id={Id}", id);
                return NotFound(new { message = "Trainee not found" });
            }
            _logger.LogInformation("Trainee found Id={Id}",id);
            return Ok(_service.MapToDto(trainee));
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

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] UpdateTraineeDto dto)
        {
            var trainee = await _service.Update(id, dto);

            if (trainee == null)
                return NotFound(new { message = "Trainee not found" });

            return Ok(_service.MapToDto(trainee));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var deleted = await _service.Delete(id);

            if (!deleted)
                return NotFound(new { message = "Trainee not found" });

            return NoContent();
        }
    }
}