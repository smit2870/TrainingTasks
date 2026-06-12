using taskmanagement.Models.DTOs.LearningTask;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;

namespace taskmanagement.Services
{
    public interface ILearningTaskService
    {
        Task<PagedResponse<LearningTask>> GetAll(string? search, LearningTaskStatus? status, int pageNumber, int pageSize);
        Task<LearningTask?> GetById(int id);
        Task<LearningTask> Create(CreateLearningTaskDto dto);
        Task<LearningTask?> Update(int id, UpdateLearningTaskDto dto);
        Task<bool> Delete(int id);
        LearningTaskResponseDto MapToDto(LearningTask task);


    }
}