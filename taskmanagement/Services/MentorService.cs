using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;
using taskmanagement.Models.DTOs.Mentor;

public class MentorService : IMentorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MentorService> _logger;

    public MentorService(AppDbContext context, ILogger<MentorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResponse<Mentor>> GetAll(string? search, MentorStatus? status, int pageNumber, int pageSize)
    {
        try
        {
            var query = _context.Mentor.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                _logger.LogInformation("Searching Mentor with keyword={Search}", search);

                search = search.ToLower();

                query = query.Where(t =>
                    t.FirstName.ToLower().Contains(search) ||
                    t.LastName.ToLower().Contains(search) ||
                    t.Email.ToLower().Contains(search) ||
                    t.Expertise.ToLower().Contains(search)
                );
            }

            if (status.HasValue)
            {
                _logger.LogInformation("Filtering mentors with status={Status}", status);
                query = query.Where(t => t.Status == status.Value);
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Fetched mentor list Count={Count}, Page={Page}, Size={Size}",
                data.Count, pageNumber, pageSize);

            return new PagedResponse<Mentor>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = data
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching mentor list");
            throw;
        }
    }
    public async Task<Mentor?> GetById(int id)
    {
        try
        {
            var mentor = await _context.Mentor.FindAsync(id);
            return mentor;
        } catch (Exception e)
        {
            _logger.LogError(e , "Error while fetching mentor Id={Id}",id);
            throw;
        }
    }

    public async Task<Mentor> Create(CreateMentorDto dto)
    {
        try
        {
            var mentor = new Mentor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Expertise = dto.Expertise,
                Status = dto.Status,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _context.Mentor.AddAsync(mentor);
            await _context.SaveChangesAsync();

             _logger.LogInformation("Mentor created successfully Id={Id}", mentor.Id);

            return mentor;
        } 
        catch(Exception e)
        {
            _logger.LogError(e, "Error while creating mentor Email={Email}",dto.Email);
            throw;
        }
    }
    public async Task<Mentor?> Update(int id, UpdateMentorDto dto)
    {
        try
        {
            var mentor = await _context.Mentor.FindAsync(id);

            if(mentor == null)
            {
                _logger.LogError("Update Failed. Mentor not found Id={Id}", id);
                return null;
            }

            _logger.LogInformation("Updating mentor ...");

            mentor.FirstName = dto.FirstName;
            mentor.LastName = dto.LastName;
            mentor.Email = dto.Email;
            mentor.Expertise = dto.Expertise;
            mentor.Status = dto.Status;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Mentor updated successfully Id={Id}",id);
            return mentor;
        }
        catch( Exception e)
        {
            _logger.LogError(e, "Error while updating Mentor Id={Id}",id);
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var mentor = await _context.Mentor.FindAsync(id);

            if(mentor == null)
            {
                 _logger.LogWarning("Delete failed. Mentor not found Id={Id}", id);
                return false;
            }

            _context.Mentor.Remove(mentor);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Mentor Deleted successfully Id={Id}", id);
            return true;
        }
        catch( Exception e)
        {
            _logger.LogError(e, "Error while Deleting Mentor Id={Id}",id);
            throw;
        }
    }

    public MentorResponseDto MapToDto(Mentor mentor)
    {
        return new MentorResponseDto
        {
            Id = mentor.Id,
            FirstName = mentor.FirstName,
            LastName = mentor.LastName,
            Email = mentor.Email,
            Status = mentor.Status,
            Expertise = mentor.Expertise,
            UpdatedDate = mentor.UpdatedDate
        };
    }
}