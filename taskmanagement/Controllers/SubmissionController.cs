using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Services;
using taskmanagement.Models.DTOs.Submission;
using taskmanagement.Models.DTOs.Common;
using taskmanagement.Options;
using Microsoft.Extensions.Options;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using System.Security.Claims;

namespace taskmanagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionController: ControllerBase
    {
        private readonly ISubmissionService _service;
        private readonly ILogger<SubmissionController> _logger;
        private readonly AppDbContext _context;
        private readonly FileValidator _validator;

        public SubmissionController(ISubmissionService service, ILogger<SubmissionController> logger, AppDbContext context, FileValidator validator)
        {
            _service = service;
            _logger = logger;
            _context = context;
            _validator = validator;
        }

        [Authorize(Roles = "Admin,Mentor")]
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

        [Authorize(Roles = "Admin,Trainee")]
        [HttpPost]
        public async Task<IActionResult> create([FromBody] CreateSubmissionDto dto)
        {
            
            var result = await _service.CreateSubmission(dto);

            return CreatedAtAction(nameof(GetById),new { id = result.Id },  _service.MapToDto(result));

        }

        [Authorize(Roles = "Admin,Trainee")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);

            if (result == null)
                return NotFound(new {message = "Submission not found."});

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Trainee")]
        [HttpPost("submission-files/{submissionId}/files")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile(int submissionId,[FromForm] UploadFileDto dto)
        {
            _validator.Validate(dto.File);
            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "system";

            try
            {
                var result = await _service.UploadFile(submissionId, dto.File, userName);

                return Created($"/api/submission-files/{result.Id}", new
                {
                    result.Id,
                    result.SubmissionId,
                    result.OriginalFileName,
                    result.ContentType,
                    result.Size,
                    result.UploadedBy,
                    result.UploadedAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Trainee")]
        [HttpGet("submission-files/{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var (stream, file) = await _service.DownloadFile(id);
                return File(stream, file.ContentType, file.OriginalFileName);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin,Trainee")]
        [HttpDelete("submission-files/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteFile(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

    }
}