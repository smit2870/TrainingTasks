using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Services;
using taskmanagement.Data;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/processing-jobs")]
    public class ProcessingJobsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProcessingJobsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _context.ProcessingJobs.FindAsync(id);

            if (job == null)
                return NotFound();

            return Ok(job);
        }
    }
}