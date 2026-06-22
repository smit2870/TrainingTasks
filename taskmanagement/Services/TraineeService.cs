using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.DTOs.Trainee;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;

namespace taskmanagement.Services
{
    public class TraineeService : ITraineeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TraineeService> _logger;
        private readonly ICacheService _cache;


        public TraineeService(AppDbContext context, ILogger<TraineeService> logger, ICacheService cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<PagedResponse<Trainee>> GetAll(string? search, TraineeStatus? status, int pageNumber, int pageSize)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching trainees list");
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

        public async Task<Trainee?> GetById(int id, int currentUserId, string role)
        {
            string cacheKey = $"trainee:{id}";
            try
            {
                var cached = await _cache.GetAsync<Trainee>(cacheKey);

                if (cached != null)
                {
                    _logger.LogInformation("Cache HIT: {Key}", cacheKey);
                    return cached;
                }
                _logger.LogInformation("Cache MISS: {Key}", cacheKey);     
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache read failed");
            }
            
            var trainee = await _context.Trainees.FindAsync(id);

            if (trainee == null)
                return null;

            if (role == "Admin" || role == "Mentor")
            {
                await _cache.SetAsync(cacheKey, trainee, TimeSpan.FromMinutes(60));
                return trainee;
            }
            
            if (role == "Trainee" && trainee.Id != currentUserId)
                throw new UnauthorizedAccessException("Access denied");

            await _cache.SetAsync(cacheKey, trainee, TimeSpan.FromMinutes(60));

            return trainee;
        }


        public async Task<Trainee?> Update(int id, UpdateTraineeDto dto, int currentUserId, string role)
        {
            var trainee = await _context.Trainees.FindAsync(id);

            if (trainee == null)
                return null;

            if (role != "Admin")
            {
                if (role == "Trainee" && trainee.Id != currentUserId)
                    throw new UnauthorizedAccessException("Access denied");

                if (role == "Mentor")
                    throw new UnauthorizedAccessException("Mentor cannot update trainees");
            }

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
            
            await _cache.RemoveAsync($"trainee:{id}");

            return trainee;
        }


        public async Task<bool> Delete(int id, int currentUserId, string role)
        {
            var trainee = await _context.Trainees.FindAsync(id);

            if (trainee == null)
                return false;

            if (role != "Admin")
                throw new UnauthorizedAccessException("Only Admin can delete trainees");

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();

            await _cache.RemoveAsync($"trainee:{id}");

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
}
