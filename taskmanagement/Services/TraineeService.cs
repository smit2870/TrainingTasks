using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.DTOs.Trainee;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;

public class TraineeService : ITraineeService
{
    private readonly AppDbContext _context;

    public TraineeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<Trainee>> GetAll(string? search, TraineeStatus? status, int pageNumber, int pageSize)
    {
        var query = _context.Trainees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();

            query = query.Where(t =>
                t.FirstName.ToLower().Contains(search) ||
                t.LastName.ToLower().Contains(search) ||
                t.Email.ToLower().Contains(search) ||
                t.TechStack.ToLower().Contains(search)
            );
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }
        var totalRecords = await query.CountAsync();

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        
        return new PagedResponse<Trainee>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            Data = data
        };

    }

    public async Task<Trainee?> GetById(int id)
    {
        return await _context.Trainees.FindAsync(id);
    }

    public async Task<Trainee> Create(AddTraineeDto dto)
    {
        var trainee = new Trainee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            TechStack = dto.TechStack,
            Status = dto.Status,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        await _context.Trainees.AddAsync(trainee);
        await _context.SaveChangesAsync();

        return trainee;
    }

    public async Task<Trainee?> Update(int id, UpdateTraineeDto dto)
    {
        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee == null) return null;

        if (!string.IsNullOrEmpty(dto.FirstName))
            trainee.FirstName = dto.FirstName;

        if (!string.IsNullOrEmpty(dto.LastName))
            trainee.LastName = dto.LastName;

        if (!string.IsNullOrEmpty(dto.Email))
            trainee.Email = dto.Email;

        if (!string.IsNullOrEmpty(dto.TechStack))
            trainee.TechStack = dto.TechStack;

        trainee.Status = dto.Status;

        trainee.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return trainee;
    }

    public async Task<bool> Delete(int id)
    {
        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee == null) return false;

        _context.Trainees.Remove(trainee);
        await _context.SaveChangesAsync();

        return true;
    }

    public TraineeResponseDto MapToDto(Trainee trainee)
    {
        return new TraineeResponseDto
        {
            Id = trainee.Id,
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email,
            TechStack = trainee.TechStack,
            Status = trainee.Status,
            UpdatedDate = trainee.UpdatedDate
        };
    }
}
