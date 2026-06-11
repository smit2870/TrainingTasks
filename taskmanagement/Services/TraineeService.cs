using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.DTOs.Trainee;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;

public class TraineeService : ITraineeService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TraineeService> _logger;

    public TraineeService(AppDbContext context, ILogger<TraineeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResponse<Trainee>> GetAll(string? search, TraineeStatus? status, int pageNumber, int pageSize)
    {
        try
        {
            var query = _context.Trainees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                _logger.LogInformation("Searching trainees with keyword={Search}", search);

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
                _logger.LogInformation("Filtering trainees with status={Status}", status);
                query = query.Where(t => t.Status == status.Value);
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Fetched trainees list Count={Count}, Page={Page}, Size={Size}",
                data.Count, pageNumber, pageSize);

            return new PagedResponse<Trainee>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = data
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching trainees list");
            throw;
        }
    }

    public async Task<Trainee?> GetById(int id)
    {
        try
        {
            var trainee = await _context.Trainees.FindAsync(id);
            return trainee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching trainee Id={Id}", id);
            throw;
        }
    }

    public async Task<Trainee> Create(AddTraineeDto dto)
    {
        try
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

            _logger.LogInformation("Trainee created successfully Id={Id}", trainee.Id);

            return trainee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating trainee Email={Email}", dto.Email);
            throw;
        }
    }

    public async Task<Trainee?> Update(int id, UpdateTraineeDto dto)
    {
        try
        {
            var trainee = await _context.Trainees.FindAsync(id);

            if (trainee == null)
            {
                _logger.LogWarning("Update failed. Trainee not found Id={Id}", id);
                return null;
            }

            _logger.LogInformation("Updating trainee Id={Id}", id);

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

            _logger.LogInformation("Trainee updated successfully Id={Id}", id);

            return trainee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating trainee Id={Id}", id);
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var trainee = await _context.Trainees.FindAsync(id);

            if (trainee == null)
            {
                _logger.LogWarning("Delete failed. Trainee not found Id={Id}", id);
                return false;
            }

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Trainee deleted successfully Id={Id}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting trainee Id={Id}", id);
            throw;
        }
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