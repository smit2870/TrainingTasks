using Microsoft.AspNetCore.Mvc;
using taskmanagement.Models;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineesController : ControllerBase
    {
        private readonly ITraineeService _service;

        public TraineesController(ITraineeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var data = await _service.GetAll(search);

            var result = data.Select(t => _service.MapToDto(t));

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trainee = await _service.GetById(id);

            if (trainee == null)
                return NotFound(new { message = "Trainee not found" });

            return Ok(_service.MapToDto(trainee));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddTraineeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newTrainee = await _service.Create(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = newTrainee.Id },_service.MapToDto(newTrainee));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] UpdateTraineeDto dto)
        {
            var trainee = await _service.Update(id, dto);

            if (trainee == null)
                return NotFound(new { message = "Trainee not found" });

            return Ok(_service.MapToDto(trainee));
        }

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