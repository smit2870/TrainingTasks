using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Mentor;
using taskmanagement.Services;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Mentor")]
    public class MentorController : ControllerBase
    {
        private readonly IMentorService _service;
        private readonly ILogger<MentorController> _logger;

        public MentorController(IMentorService service, ILogger<MentorController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? search, MentorStatus? status, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("pageNumber and pageSize must be greater than 0");

            var result = await _service.GetAll(search, status, pageNumber, pageSize);
            var mapped = result.Data.Select(t => _service.MapToDto(t)).ToList();

            return Ok(new
            {
                pageNumber = result.PageNumber,
                pageSize = result.PageSize,
                totalRecords = result.TotalRecords,
                data = mapped
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var mentor = await _service.GetById(id, userId, role!);

            if (mentor == null)
                return NotFound(new { message = "Mentor not found" });

            return Ok(_service.MapToDto(mentor));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateMentorDto dto)
        {
            var mentor = await _service.Create(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = mentor.Id },
                _service.MapToDto(mentor));
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, UpdateMentorDto dto)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var mentor = await _service.Update(id, dto, userId, role!);

            if (mentor == null)
                return NotFound(new { message = "Mentor not found" });

            return Ok(_service.MapToDto(mentor));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var deleted = await _service.Delete(id, userId, role!);

            if (!deleted)
                return NotFound(new { message = "Mentor not found" });

            return NoContent();
        }
    }
}