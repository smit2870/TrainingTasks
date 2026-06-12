using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.DTOs.LearningTask;
using taskmanagement.Models.DTOs.Common;
using taskmanagement.Models.Enums;

namespace taskmanagement.Services
{
    public class LearningTaskService : ILearningTaskService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LearningTaskService> _logger;

        public LearningTaskService(AppDbContext context, ILogger<LearningTaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResponse<LearningTask>> GetAll(string? search, LearningTaskStatus? status, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.LearningTask.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    _logger.LogInformation("Searching learning tasks with keyword={Search}", search);

                    search = search.ToLower();

                    query = query.Where(t =>
                        t.Title.ToLower().Contains(search) ||
                        t.Description.ToLower().Contains(search) ||
                        t.ExpectedTechStack.ToLower().Contains(search)
                    );
                }

                if (status.HasValue)
                {
                    _logger.LogInformation("Filtering learning tasks with status={Status}", status);
                    query = query.Where(t => t.Status == status.Value);
                }

                var totalRecords = await query.CountAsync();

                var data = await query
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Fetched learning tasks Count={Count}, Page={Page}, Size={Size}",
                    data.Count, pageNumber, pageSize);

                return new PagedResponse<LearningTask>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching learning tasks list");
                throw;
            }
        }

        public async Task<LearningTask?> GetById(int id)
        {
            try
            {
                var task = await _context.LearningTask.FindAsync(id);
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching learning task Id={Id}", id);
                throw;
            }
        }

        public async Task<LearningTask> Create(CreateLearningTaskDto dto)
        {
            try
            {
                var task = new LearningTask
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ExpectedTechStack = dto.ExpectedTechStack,
                    DueDate = dto.DueDate,
                    Status = dto.Status,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _context.LearningTask.AddAsync(task);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Learning task created successfully Id={Id}", task.Id);

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating learning task Title={Title}", dto.Title);
                throw;
            }
        }

        public async Task<LearningTask?> Update(int id, UpdateLearningTaskDto dto)
        {
            try
            {
                var task = await _context.LearningTask.FindAsync(id);

                if (task == null)
                {
                    _logger.LogWarning("Update failed. Learning task not found Id={Id}", id);
                    return null;
                }

                _logger.LogInformation("Updating learning task Id={Id}", id);

                if (!string.IsNullOrEmpty(dto.Title))
                    task.Title = dto.Title;

                if (!string.IsNullOrEmpty(dto.Description))
                    task.Description = dto.Description;

                if (!string.IsNullOrEmpty(dto.ExpectedTechStack))
                    task.ExpectedTechStack = dto.ExpectedTechStack;
                
                task.DueDate = dto.DueDate;

                task.Status = dto.Status;
                task.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Learning task updated successfully Id={Id}", id);

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating learning task Id={Id}", id);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var task = await _context.LearningTask.FindAsync(id);

                if (task == null)
                {
                    _logger.LogWarning("Delete failed. Learning task not found Id={Id}", id);
                    return false;
                }

                _context.LearningTask.Remove(task);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Learning task deleted successfully Id={Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting learning task Id={Id}", id);
                throw;
            }
        }

        public LearningTaskResponseDto MapToDto(LearningTask task)
        {
            return new LearningTaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                ExpectedTechStack = task.ExpectedTechStack,
                DueDate = task.DueDate,
                Status = task.Status,
                UpdatedDate = task.UpdatedDate
            };
        }
    }
}