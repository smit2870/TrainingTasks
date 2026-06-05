using Microsoft.AspNetCore.Mvc;
using taskmanagement.Models;

namespace taskmanagement.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineesController : ControllerBase{

        private readonly ITraineeService _service;

        public TraineesController(ITraineeService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll(){
            return Ok(_service.GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById(int id){
            var trainee = _service.GetById(id);

            if (trainee == null)
                return NotFound(new { message = "Trainee not found" });

            return Ok(_service.GetResponseData(trainee));
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddTraineeDto newTraineeDto){
            var newTrainee = _service.Create(newTraineeDto);

            return CreatedAtAction( nameof(GetById), new { id = newTrainee.Id }, newTrainee);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult updateById(int id, [FromBody] UpdateTraineeDto updateTraineeDto)
        {
            var trainee = _service.Update(id, updateTraineeDto);

            if (trainee == null)
            {
                return NotFound(new { message = "Trainee not found" });
            }

            return Ok(_service.GetResponseData(trainee));
            
        }

        [HttpDelete()]
        [Route("{id}")]
        public IActionResult deleteById(int id){
             var deleted = _service.Delete(id);

            if (!deleted)
                return NotFound(new { message = "Trainee not found" });

            return NoContent();
        }

    }

    
}