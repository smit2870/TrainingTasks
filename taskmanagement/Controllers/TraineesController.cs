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
        public IActionResult GetAll()
        {
            var data = _service.GetAll().ToList();
            // var data = _service.GetAll()
            //     .Select(t => _service.MapToDto(t));

            return Ok(data);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var trainee = _service.GetById(id);

            if (trainee == null)
                return NotFound(new { message = "Trainee not found" });

            return Ok(_service.MapToDto(trainee));
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddTraineeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newTrainee = _service.Create(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = newTrainee.Id },
                _service.MapToDto(newTrainee));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateById(int id, [FromBody] UpdateTraineeDto dto)
        {
            var trainee = _service.Update(id, dto);

            if (trainee == null)
                return NotFound(new { message = "Trainee not found" });

            return Ok(_service.MapToDto(trainee));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            var deleted = _service.Delete(id);

            if (!deleted)
                return NotFound(new { message = "Trainee not found" });

            return NoContent();
        }
    }
}