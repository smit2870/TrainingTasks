using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using taskmanagement.Models.DTOs.Common;
using taskmanagement.Models.DTOs.TaskAssignment;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Services;
using System.Security.Claims;

namespace taskmanagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskAssignmentsController : ControllerBase
    {
        private readonly ITaskAssignmentService _service;
        private readonly ILogger<TaskAssignmentsController> _logger;

        public TaskAssignmentsController(ITaskAssignmentService service, ILogger<TaskAssignmentsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? traineeId, [FromQuery] int? mentorId, [FromQuery] TaskAssignmentStatus? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _service.GetAll(traineeId, mentorId, status, pageNumber, pageSize);

                var response = new PagedResponse<TaskAssignmentResponseDto>
                {
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalRecords = result.TotalRecords,
                    Data = result.Data.Select(x => _service.MapToDto(x)).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching task assignments");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin,Mentor,Trainee")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || string.IsNullOrEmpty(role))
                {
                    return Unauthorized("Invalid user token");
                }

                var entity = await _service.GetById(id, userId, role!);

                if (entity == null)
                    return NotFound($"TaskAssignment not found with Id={id}");

                return Ok(entity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching TaskAssignment Id={Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskAssignmentDto dto)
        {
            try
            {
                var created = await _service.Create(dto);

                var response = _service.MapToDto(created);

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating TaskAssignment");
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskAssignmentDto dto)
        {
            try
            {
                var updated = await _service.UpdateStatus(id, dto);

                if (updated == null)
                    return NotFound($"TaskAssignment not found with Id={id}");

                return Ok(_service.MapToDto(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating TaskAssignment Id={Id}", id);
                return BadRequest(ex.Message);
            }
        }
    }
}