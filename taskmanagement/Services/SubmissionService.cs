using Microsoft.EntityFrameworkCore;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Submission;
using taskmanagement.Models.Messaging;
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
        private readonly ICacheService _cache;
        private readonly IAuthorizationService _authService;
        private readonly IRabbitMqPublisher _publisher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger, IFileStorageService storage, 
                FileValidator validator, ICacheService cache, IAuthorizationService authorization, IRabbitMqPublisher publisher, 
                IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _storage = storage;
            _validator = validator;
            _cache = cache;
            _authService = authorization;
            _publisher = publisher;
            _httpContextAccessor = httpContextAccessor;
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

                    await _cache.RemoveAsync($"submission:{existing.Id}");
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

                await _cache.RemoveAsync($"submission:{submission.Id}");
                return submission;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while submitting task.");
                throw;
            }
        }

        public async Task<IEnumerable<Submission>> GetAll()
        {
            try
            {
                return await _context.Submission.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while Getting task submission data.");
                throw;
            }
        }
        public async Task<SubmissionResponseDto?> GetById(int id, int userId, string role)
        {
            await _authService.CheckSubmissionAccess(id, userId, role);
            string cacheKey = $"submission:{id}";

            try
            {
                var cached = await _cache.GetAsync<SubmissionResponseDto>(cacheKey);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT: {Key}", cacheKey);
                    return cached;
                }

                _logger.LogInformation("Cache MISS: {Key}", cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache read failed");
            }

            var submission = await _context.Submission.FindAsync(id);
            var dto = MapToDto(submission);
            if (submission == null)
                return null;

            try
            {
                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(60));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache set failed");
            }

            return dto;

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

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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

                var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();

                var message = new SubmissionProcessingRequested
                {
                    MessageId = Guid.NewGuid(),
                    CorrelationId = Guid.Parse(correlationId),
                    SubmissionId = submissionId,
                    FileId = entity.Id,
                    RequestedAt = DateTime.UtcNow
                };

                var processingJobs = new ProcessingJob
                {
                    MessageId = message.MessageId,
                    CorrelationId = message.CorrelationId,
                    SubmissionId = submissionId,
                    FileId = entity.Id,
                    Status = JobStatus.Queued,
                    Attempts = 0,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ProcessingJobs.Add(processingJobs);
                await _context.SaveChangesAsync();

                var published = await _publisher.PublishAsync(message);

                if (!published)
                {
                    _logger.LogWarning("Message NOT published for SubmissionId: {SubmissionId}", submissionId);
                    throw new Exception("Failed to enqueue processing job");
                }

                await transaction.CommitAsync();

                using (_logger.BeginScope("CorrelationId:{CorrelationId}", message.CorrelationId))
                {    
                    _logger.LogInformation("Queued processing - MsgId: {MsgId}, SubId: {SubId}, FileId: {FileId}, --  CorrelationId: {CorrelationId}",message.MessageId, submissionId, entity.Id, correlationId);
                }

                await _cache.RemoveAsync($"submission:{submissionId}");
                return entity;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "UploadFile failed. Rolling back transaction.");

                try      
                {            
                    await _storage.DeleteAsync(storageName);
                }        
                catch (Exception cleanupEx)        
                {            
                    _logger.LogError(cleanupEx, "Failed to cleanup uploaded file after rollback");        
                }        
                throw new Exception(" *** Processing temporarily unavailable. Please try again.");
            }
        }

        public async Task<(Stream stream, SubmissionFile file)> DownloadFile(int fileId, int userId, string role)
        {
            await _authService.CheckFileAccess(fileId, userId, role);

            var file = await _context.SubmissionFiles.FindAsync(fileId);
            if (file == null)
                throw new Exception("File not found");

            if (!await _storage.ExistsAsync(file.StorageName))
                throw new Exception("File missing in storage");

            var stream = await _storage.OpenReadAsync(file.StorageName);

            return (stream, file);
        }

        public async Task DeleteFile(int fileId, int userId, string role)
        {
            await _authService.CheckFileAccess(fileId, userId, role);
            
            var file = await _context.SubmissionFiles.FindAsync(fileId);
            if (file == null)
                throw new Exception("File not found");

            await _storage.DeleteAsync(file.StorageName);

            _context.SubmissionFiles.Remove(file);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"submission:{file.SubmissionId}");
        }
    }
}