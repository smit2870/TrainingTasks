using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Models.DTOs.LearningTask;
using taskmanagement.Models.Enums;
using taskmanagement.Services;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningTasksController : ControllerBase
    {
        private readonly ILearningTaskService _service;
        private readonly ILogger<LearningTasksController> _logger;

        public LearningTasksController(ILearningTaskService service, ILogger<LearningTasksController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Mentor,Trainee")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] LearningTaskStatus? status,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Invalid pagination params PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);

                return BadRequest("pageNumber and pageSize must be greater than 0");
            }

            var result = await _service.GetAll(search, status, pageNumber, pageSize);

            var mapped = result.Data.Select(t => _service.MapToDto(t)).ToList();

            _logger.LogInformation("Learning tasks fetched Page={Page}, Size={Size}, Count={Count}",pageNumber, pageSize, mapped.Count);

            return Ok(new
            {
                pageNumber = result.PageNumber,
                pageSize = result.PageSize,
                totalRecords = result.TotalRecords,
                data = mapped
            });
        }

        [Authorize(Roles = "Admin,Mentor,Trainee")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _service.GetById(id);

            if (task == null)
            {
                _logger.LogWarning("Learning task not found Id={Id}", id);
                return NotFound(new { message = "Learning task not found" });
            }

            _logger.LogInformation("Learning task found Id={Id}", id);

            return Ok(_service.MapToDto(task));
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLearningTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid learning task creation request");
                return BadRequest(ModelState);
            }

            var newTask = await _service.Create(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = newTask.Id },
                _service.MapToDto(newTask));
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] UpdateLearningTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid learning task update request Id={Id}", id);
                return BadRequest(ModelState);
            }

            var updatedTask = await _service.Update(id, dto);

            if (updatedTask == null)
                return NotFound(new { message = "Learning task not found" });

            return Ok(_service.MapToDto(updatedTask));
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var deleted = await _service.Delete(id);

            if (!deleted)
                return NotFound(new { message = "Learning task not found" });

            return NoContent();
        }
    }
}