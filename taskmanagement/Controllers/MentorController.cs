using Microsoft.AspNetCore.Mvc;
using taskmanagement.Models.DTOs.Trainee;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Mentor;
using taskmanagement.Services;

namespace taskmanagement.Controllers
{
    [Authorize(Roles = "Admin,Mentor")]
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] MentorStatus? status, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Invalid pagination params PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);

                return BadRequest("pageNumber and pageSize must be greater than 0");
            }

            var result = await _service.GetAll(search,status,pageNumber,pageSize);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var mentor = await _service.GetById(id);

            if(mentor == null)
            {
                _logger.LogWarning("Mentor not found Id={Id}",id);
                return NotFound(new { message = "Mentor not found"});
            }
            _logger.LogInformation("Mentor not found Id={Id}",id);
            return Ok(_service.MapToDto(mentor));
        }

        [HttpPost]
        public async Task<IActionResult> create([FromBody] CreateMentorDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid trainee creation request");
                return BadRequest(ModelState);
            }

            var mentor = await _service.Create(dto);

            return CreatedAtAction(nameof(GetById), new {id = mentor.Id},_service.MapToDto(mentor));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] UpdateMentorDto dto)
        {
            var mentor = await _service.Update(id, dto);

            if(mentor == null)
            {
                return NotFound(new { message = "Mentor not found"});    
            }

            return Ok(_service.MapToDto(mentor));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var mentor = await _service.Delete(id);

            if(mentor == false)
            {
                return NotFound(new { message = "Mentor not found"});
            }

            return NoContent();
        }
    }
}