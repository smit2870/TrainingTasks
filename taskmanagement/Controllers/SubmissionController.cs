using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Services;
using taskmanagement.Models.DTOs.Submission;


namespace taskmanagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionController: ControllerBase
    {
        private readonly ISubmissionService _service;
        private readonly ILogger<SubmissionController> _logger;

        public SubmissionController(ISubmissionService service, ILogger<SubmissionController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var submissions = await _service.GetAll();

            var mapped = submissions
                .Select(s => _service.MapToDto(s))
                .ToList();

            _logger.LogInformation("Fetched {Count} submissions", mapped.Count);

            return Ok(mapped);
        }

        [HttpPost]
        public async Task<IActionResult> create([FromBody] CreateSubmissionDto dto)
        {
            
            var result = await _service.CreateSubmission(dto);

            return CreatedAtAction(nameof(GetById),new { id = result.Id },  _service.MapToDto(result));

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);

            if (result == null)
                return NotFound(new {message = "Submission not found."});

            return Ok(_service.MapToDto(result));
        }

    }
}