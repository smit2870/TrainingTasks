using Microsoft.EntityFrameworkCore;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Submission;
using taskmanagement.Data;

namespace taskmanagement.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SubmissionService> _logger;
        
        public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Submission> CreateSubmission(CreateSubmissionDto dto)
        {
            try
            {
                var assignmentExists = await _context.TaskAssignment.AnyAsync(x => x.Id == dto.TaskAssignmentId);

                if (!assignmentExists)
                {
                    throw new Exception("Invalid TaskAssignmentId. Assignment does not exist.");
                }

                var existing = await _context.Submission.FirstOrDefaultAsync(x => x.TaskAssignmentId == dto.TaskAssignmentId);

                if (existing != null)
                {
                    existing.SubmissionUrl = dto.SubmissionUrl;
                    existing.Notes = dto.Notes;
                    existing.SubmittedDate = DateTime.UtcNow;
                    existing.Status = SubmissionStatus.Resubmitted;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Assignment Task Resubmitted successfully");
                    return existing;
                }
                
                var submission = new Submission
                {
                    TaskAssignmentId = dto.TaskAssignmentId,
                    SubmissionUrl = dto.SubmissionUrl,
                    Notes = dto.Notes,
                    SubmittedDate = DateTime.UtcNow,
                    Status = SubmissionStatus.Submitted
                };

                _context.Submission.Add(submission);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Assignment Task submitted successfully.");

                return submission;
            }
            catch(Exception e)
            {
                _logger.LogError(e,"Error while submitting task.");
                throw;
            }
        }

        public async Task<IEnumerable<Submission>> GetAll()
        {
            try
            {
                return await _context.Submission.ToListAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e,"Error while Getting task submission data.");
                throw;
            }
        }
        public async Task<Submission> GetById(int id)
        {
            try
            {
                return await _context.Submission.FindAsync(id);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error while Getting task submission Id={Id}",id);
                throw;
            }
        }

 
        public SubmissionResponseDto MapToDto(Submission s)
        {
            return new SubmissionResponseDto
            {
                Id = s.Id,
                TaskAssignmentId = s.TaskAssignmentId,
                SubmissionUrl = s.SubmissionUrl,
                Notes = s.Notes,
                SubmittedDate = s.SubmittedDate,
                Status = s.Status
            };
        }
    }
}