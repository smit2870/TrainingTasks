using Microsoft.EntityFrameworkCore;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Submission;
using taskmanagement.Data;
using System.Security.Cryptography;

namespace taskmanagement.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SubmissionService> _logger;
        private readonly IFileStorageService _storage;
        private readonly FileValidator _validator;
        
        public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger, IFileStorageService storage, FileValidator validator)
        {
            _context = context;
            _logger = logger;
            _storage = storage;
            _validator = validator;
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
        public async Task<Submission?> GetById(int id)
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

        public async Task<SubmissionFile> UploadFile(int submissionId, IFormFile file, string userName)
            {
                _validator.Validate(file);
                
                var submission = await _context.Submission.FindAsync(submissionId);
                if (submission == null)
                    throw new Exception("Submission not found");

                string storageName;
                using (var stream = file.OpenReadStream())
                {
                    storageName = await _storage.SaveAsync(stream, file.ContentType);
                }

                using var sha256 = SHA256.Create();
                using var fs = file.OpenReadStream();
                var hash = Convert.ToHexString(await sha256.ComputeHashAsync(fs));

                var entity = new SubmissionFile
                {
                    SubmissionId = submissionId,
                    OriginalFileName = file.FileName,
                    StorageName = storageName,
                    ContentType = file.ContentType,
                    Size = file.Length,
                    Checksum = hash,
                    UploadedBy = userName,
                    UploadedAt = DateTime.UtcNow
                };

                _context.SubmissionFiles.Add(entity);
                await _context.SaveChangesAsync();

                return entity;
            }

            public async Task<(Stream stream, SubmissionFile file)> DownloadFile(int fileId)
            {
                var file = await _context.SubmissionFiles.FindAsync(fileId);
                if (file == null)
                    throw new Exception("File not found");

                if (!await _storage.ExistsAsync(file.StorageName))
                    throw new Exception("File missing in storage");

                var stream = await _storage.OpenReadAsync(file.StorageName);

                return (stream, file);
            }

            public async Task DeleteFile(int fileId)
            {
                var file = await _context.SubmissionFiles.FindAsync(fileId);
                if (file == null)
                    throw new Exception("File not found");

                await _storage.DeleteAsync(file.StorageName);

                _context.SubmissionFiles.Remove(file);
                await _context.SaveChangesAsync();
    }

    }
}