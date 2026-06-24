using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.DTOs.Common;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;

namespace taskmanagement.Services;

public class ProcessingJobService : IProcessingJobService
{
    private readonly AppDbContext _context;

    public ProcessingJobService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProcessingJobDto?> GetByIdAsync(int id)
    {
        var job = await _context.ProcessingJobs.FirstOrDefaultAsync(x => x.Id == id);

        if (job == null)
            return null;

        return MapToDto(job);
    }

    private ProcessingJobDto MapToDto(ProcessingJob job)
    {
        return new ProcessingJobDto
        {
            Id = job.Id,
            MessageId = job.MessageId,
            CorrelationId = job.CorrelationId,
            SubmissionId = job.SubmissionId,
            FileId = job.FileId,
            Status = job.Status,
            Attempts = job.Attempts,
            ErrorSummary = job.ErrorSummary,
            Result = job.Result,
            CreatedAt = job.CreatedAt,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt
        };
    }

    public async Task<IEnumerable<ProcessingJobDto>> GetByStatusAsync(JobStatus status)
    {
        var jobs = await _context.ProcessingJobs
            .Where(x => x.Status == status)
            .ToListAsync();

        return jobs.Select(MapToDto);
    }

}