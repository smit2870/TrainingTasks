using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using taskmanagement.Services;

namespace SubmissionProcessor.Services;

public class SubmissionProcessingService : ISubmissionProcessingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionProcessingService> _logger;
    private readonly IFileStorageService _storage;

    public SubmissionProcessingService(AppDbContext context, ILogger<SubmissionProcessingService> logger, IFileStorageService storage)
    {
        _context = context;
        _logger = logger;
        _storage = storage;
    }

    public async Task ProcessAsync(SubmissionProcessingRequested message, CancellationToken cancellationToken)
    {
        var job = await _context.ProcessingJobs.FirstOrDefaultAsync(x => x.MessageId == message.MessageId, cancellationToken);

        if (job == null)
            throw new Exception(" *** Processing job not found");

        if (job.FileId == message.FileId && job.Status == JobStatus.Completed)
        {
            _logger.LogInformation("  ***  Duplicate ignored: {MessageId}", message.MessageId);
            return;
        }

        try
        {
            if (job.Attempts >= 3)
            {
                job.Status = JobStatus.Failed;
                job.ErrorSummary = "Max retry attempts reached";
                job.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogError("  ***  Retry limit reached for MsgId: {MessageId}", message.MessageId);

                throw new Exception("Retry limit exceeded");
            }

            job.Status = JobStatus.Processing;
            job.StartedAt = DateTime.UtcNow;
            job.Attempts += 1; 

            await _context.SaveChangesAsync(cancellationToken);

            var file = await _context.SubmissionFiles.FirstOrDefaultAsync(x => x.Id == message.FileId, cancellationToken);

            if (file == null)
                throw new Exception("Submission file not found");

            if (!await _storage.ExistsAsync(file.StorageName))
                throw new Exception("File missing in storage");

            await using var stream = await _storage.OpenReadAsync(file.StorageName);

            using var sha = SHA256.Create();

            var hash = await sha.ComputeHashAsync(stream, cancellationToken);

            var checksum = Convert.ToHexString(hash);

            job.Result = checksum;

            file.Checksum = checksum;
            job.Status = JobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(" --- Processing completed for MsgId: {MessageId}, FileId: {FileId} ", message.MessageId, message.FileId);

        }
        catch (Exception ex)
        {
            job.Status = JobStatus.Failed;
            job.ErrorSummary = ex.Message;

            await _context.SaveChangesAsync(cancellationToken);
            throw;
        }
    }
}