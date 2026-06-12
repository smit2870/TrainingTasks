using taskmanagement.Models.DTOs.TaskAssignment;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;

namespace taskmanagement.Services
{
    public interface ITaskAssignmentService
    {
        Task<PagedResponse<TaskAssignment>> GetAll(int? traineeId, int? mentorId, TaskAssignmentStatus? status, int pageNumber, int pageSize);

        Task<TaskAssignment?> GetById(int id);

        Task<TaskAssignment> Create(CreateTaskAssignmentDto dto);

        Task<TaskAssignment?> UpdateStatus(int id, UpdateTaskAssignmentDto dto);

        TaskAssignmentResponseDto MapToDto(TaskAssignment entity);
    }
}

