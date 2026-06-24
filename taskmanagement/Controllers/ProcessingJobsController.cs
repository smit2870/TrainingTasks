using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Services;
using taskmanagement.Data;
using taskmanagement.Models.Enums;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/processing-jobs")]
    public class ProcessingJobsController : ControllerBase
    {
        private readonly IProcessingJobService _service;

        public ProcessingJobsController(IProcessingJobService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _service.GetByIdAsync(id);

            if (job == null)
                return NotFound();

            return Ok(job);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(JobStatus status)
        {
            var jobs = await _service.GetByStatusAsync(status);

            return Ok(jobs);
        }

    }
}