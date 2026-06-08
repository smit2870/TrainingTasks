using taskmanagement.Data;
using taskmanagement.Models;

public class TraineeService : ITraineeService
{
    // private static readonly List<Trainee> trainees = new()
    // {
    //     new Trainee
    //     {
    //         Id = 1,
    //         FirstName = "Smit",
    //         LastName = "Patel",
    //         Email = "smit@example.com",
    //         TechStack = "DotNet",
    //         Status = TraineeStatus.Available,
    //         CreatedDate = DateTime.UtcNow,
    //         UpdatedDate = DateTime.UtcNow
    //     }
    // };

    private readonly AppDbContext _context;

    public TraineeService(AppDbContext context)
    {
        _context = context;
    }

    public List<Trainee> GetAll()
    {
        return _context.Trainees.ToList();
    }

    public Trainee? GetById(int id)
    {
        return _context.Trainees.FirstOrDefault(t => t.Id == id);
    }

    public Trainee Create(AddTraineeDto dto)
    {
        var id = _context.Trainees.Any() ? _context.Trainees.Max(t => t.Id) + 1 : 1;

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

        _context.Trainees.Add(trainee);
        _context.SaveChanges();
        return trainee;
    }

    public Trainee? Update(int id, UpdateTraineeDto dto)
    {
        var trainee = _context.Trainees.FirstOrDefault(t => t.Id == id);
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
         _context.SaveChanges();
        return trainee;
    }

    public bool Delete(int id)
    {
        var trainee = _context.Trainees.FirstOrDefault(t => t.Id == id);
        if (trainee == null) return false;

        _context.Trainees.Remove(trainee);
        _context.SaveChanges();
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
