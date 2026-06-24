using taskmanagement.Models.DTOs.Common;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;

namespace taskmanagement.Services;

public interface IProcessingJobService
{
    Task<ProcessingJobDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProcessingJobDto>> GetByStatusAsync(JobStatus status);
}