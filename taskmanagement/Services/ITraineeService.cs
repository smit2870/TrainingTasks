using taskmanagement.Models;

public interface ITraineeService
{
    List<Trainee> GetAll();
    Trainee? GetById(int id);
    Trainee Create(AddTraineeDto dto);
    Trainee? Update(int id, UpdateTraineeDto dto);
    bool Delete(int id);
    TraineeResponseDto MapToDto(Trainee trainee);
}