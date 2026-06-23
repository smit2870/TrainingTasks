using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;
using taskmanagement.Models.DTOs.TaskAssignment;

namespace taskmanagement.Services
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskAssignmentService> _logger;
        private readonly TaskAssignmentValidator _validator;
        private readonly ICacheService _cache;

        public TaskAssignmentService(AppDbContext context, ILogger<TaskAssignmentService> logger, TaskAssignmentValidator validator, ICacheService cache)
        {
            _context = context;
            _logger = logger;
            _validator = validator;
            _cache = cache;
        }

        public async Task<PagedResponse<TaskAssignment>> GetAll(int? traineeId, int? mentorId, TaskAssignmentStatus? status, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.TaskAssignment.AsQueryable();

                if (traineeId.HasValue)
                {
                    _logger.LogInformation("Filtering by TraineeId={TraineeId}", traineeId);
                    query = query.Where(x => x.TraineeId == traineeId.Value);
                }

                if (mentorId.HasValue)
                {
                    _logger.LogInformation("Filtering by MentorId={MentorId}", mentorId);
                    query = query.Where(x => x.MentorId == mentorId.Value);
                }

                if (status.HasValue)
                {
                    _logger.LogInformation("Filtering by Status={Status}", status);
                    query = query.Where(x => x.Status == status.Value);
                }

                var totalRecords = await query.CountAsync();

                var data = await query
                    .OrderByDescending(x => x.AssignedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Fetched TaskAssignments Count={Count}, Page={Page}, Size={Size}",
                    data.Count, pageNumber, pageSize);

                return new PagedResponse<TaskAssignment>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching TaskAssignments");
                throw;
            }
        }

        public async Task<TaskAssignmentResponseDto?> GetById(int id)
        {
            string cacheKey = $"task-assignment:{id}";
            try
            {
                var cached = await _cache.GetAsync<TaskAssignmentResponseDto>(cacheKey);

                if(cached != null)
                {
                    _logger.LogInformation("Cache HIT: {Key}", cacheKey);
                    return cached;
                }
                _logger.LogInformation("Cache MISS: {Key}", cacheKey); 

                var assignment = await _context.TaskAssignment.FindAsync(id);
                var dto = MapToDto(assignment);
                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(60));
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching TaskAssignment Id={Id}", id);
                throw;
            }
        }

        public async Task<TaskAssignment> Create(CreateTaskAssignmentDto dto)
        {
            try
            {
                await _validator.ValidateCreateAsync(dto);

                var assignment = new TaskAssignment
                {
                    TraineeId = dto.TraineeId,
                    MentorId = dto.MentorId,
                    LearningTaskId = dto.LearningTaskId,
                    AssignedDate = dto.AssignedDate,
                    DueDate = dto.DueDate,
                    Status = TaskAssignmentStatus.Assigned,
                    Remarks = dto.Remarks
                };

                await _context.TaskAssignment.AddAsync(assignment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("TaskAssignment created successfully Id={Id}", assignment.Id);

                return assignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while creating TaskAssignment TraineeId={TraineeId}, MentorId={MentorId}",
                    dto.TraineeId, dto.MentorId);
                throw;
            }
        }

        public async Task<TaskAssignment?> UpdateStatus(int id, UpdateTaskAssignmentDto dto)
        {
            try
            {
                var assignment = await _context.TaskAssignment.FindAsync(id);

                if (assignment == null)
                {
                    _logger.LogWarning("Update failed. TaskAssignment not found Id={Id}", id);
                    return null;
                }

                _logger.LogInformation("Updating TaskAssignment Status Id={Id}", id);
                
                _validator.ValidateStatusUpdate(assignment, dto);

                assignment.Status = dto.Status;
                assignment.Remarks = dto.Remarks;

                await _context.SaveChangesAsync();
                await _cache.RemoveAsync($"task-assignment:{id}");

                _logger.LogInformation("TaskAssignment status updated Id={Id}", id);

                return assignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating TaskAssignment Id={Id}", id);
                throw;
            }
        }

        public TaskAssignmentResponseDto MapToDto(TaskAssignment assignment)
        {
            return new TaskAssignmentResponseDto
            {
                Id = assignment.Id,
                TraineeId = assignment.TraineeId,
                MentorId = assignment.MentorId,
                LearningTaskId = assignment.LearningTaskId,
                AssignedDate = assignment.AssignedDate,
                DueDate = assignment.DueDate,
                Status = assignment.Status,
                Remarks = assignment.Remarks
            };
        }
    }
}