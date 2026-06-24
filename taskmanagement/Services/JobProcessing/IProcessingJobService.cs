using taskmanagement.Models.DTOs.Common;
using taskmanagement.Models.Entities;

namespace taskmanagement.Services;

public interface IProcessingJobService
{
    Task<ProcessingJobDto?> GetByIdAsync(int id);
}