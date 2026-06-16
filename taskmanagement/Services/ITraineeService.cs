using taskmanagement.Models.DTOs.Trainee;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;

namespace taskmanagement.Services
{
    public interface ITraineeService
    {
        Task<PagedResponse<Trainee>> GetAll(string? search, TraineeStatus? status, int pageNumber, int pageSize);
        Task<Trainee?> GetById(int id, int currentUserId, string role);
        Task<Trainee> Create(AddTraineeDto dto);
        Task<Trainee?> Update(int id, UpdateTraineeDto dto, int currentUserId, string role);
        Task<bool> Delete(int id, int currentUserId, string role);
        TraineeResponseDto MapToDto(Trainee trainee);
    }
}

