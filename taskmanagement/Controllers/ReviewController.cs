using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Services;
using taskmanagement.Models.DTOs.Review;


namespace taskmanagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController: ControllerBase
    {
        private readonly IRerviewService _service;
        private readonly ILogger<SubmissionController> _logger;

        public ReviewController(IRerviewService service, ILogger<SubmissionController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _service.GetAll();

            var mapped = reviews
                .Select(r => _service.MapToDto(r))
                .ToList();

            _logger.LogInformation("Fetched {Count} Reviews", mapped.Count);

            return Ok(mapped);
        }

        [HttpPost]
        public async Task<IActionResult> create([FromBody] CreateReviewDto dto)
        {
            
            var result = await _service.Create(dto);

            return CreatedAtAction(nameof(GetById),new { id = result.Id },  _service.MapToDto(result));

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);

            if (result == null)
                return NotFound(new {message = "Review not found."});

            return Ok(_service.MapToDto(result));
        }

    }
}