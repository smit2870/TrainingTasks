using taskmanagement.Models;

public class TraineeService : ITraineeService
{
    private static readonly List<Trainee> trainees = new()
    {
        new Trainee
        {
            Id = 1,
            FirstName = "Smit",
            LastName = "Patel",
            Email = "smit@example.com",
            TechStack = "DotNet",
            Status = TraineeStatus.Available,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        }
    };

    public List<Trainee> GetAll()
    {
        return trainees;
    }

    public Trainee? GetById(int id)
    {
        return trainees.FirstOrDefault(t => t.Id == id);
    }

    public Trainee Create(AddTraineeDto dto)
    {
        var id = trainees.Any() ? trainees.Max(t => t.Id) + 1 : 1;

        var trainee = new Trainee
        {
            Id = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            TechStack = dto.TechStack,
            Status = dto.Status,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        trainees.Add(trainee);
        return trainee;
    }

    public Trainee? Update(int id, UpdateTraineeDto dto)
    {
        var trainee = trainees.FirstOrDefault(t => t.Id == id);
        if (trainee == null) return null;

        if (!string.IsNullOrEmpty(dto.FirstName))
            trainee.FirstName = dto.FirstName;

        if (!string.IsNullOrEmpty(dto.LastName))
            trainee.LastName = dto.LastName;

        if (!string.IsNullOrEmpty(dto.Email))
            trainee.Email = dto.Email;

        if (!string.IsNullOrEmpty(dto.TechStack))
            trainee.TechStack = dto.TechStack;

        if (dto.Status.HasValue)
            trainee.Status = dto.Status.Value;

        trainee.UpdatedDate = DateTime.UtcNow;

        return trainee;
    }

    public bool Delete(int id)
    {
        var trainee = trainees.FirstOrDefault(t => t.Id == id);
        if (trainee == null) return false;

        trainees.Remove(trainee);
        return true;
    }

    public TraineeResponseDto MapToDto(Trainee trainee)
    {
        return new TraineeResponseDto
        {
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email,
            TechStack = trainee.TechStack,
            Status = trainee.Status,
            UpdatedDate = trainee.UpdatedDate
        };
    }
}
