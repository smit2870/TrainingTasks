using taskmanagement.Models;

public interface ITraineeService
{
    Task<List<Trainee>> GetAll(string? search = null);
    Task<Trainee?> GetById(int id);

    Task<Trainee> Create(AddTraineeDto dto);
    Task<Trainee?> Update(int id, UpdateTraineeDto dto);

    Task<bool> Delete(int id);

    TraineeResponseDto MapToDto(Trainee trainee);
}
