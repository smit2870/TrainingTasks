using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;
using taskmanagement.Models.DTOs.Mentor;

namespace taskmanagement.Services
{
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
            var query = _context.Mentor.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
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
                query = query.Where(t => t.Status == status.Value);
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Mentor>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = data
            };
        }

        public async Task<Mentor?> GetById(int id, int currentUserId, string role)
        {
            var mentor = await _context.Mentor.FindAsync(id);

            if (mentor == null)
                return null;

            if (role == "Admin")
                return mentor;

            if (role == "Mentor" && mentor.Id != currentUserId)
                throw new UnauthorizedAccessException("Access denied");

            return mentor;
        }

        public async Task<Mentor> Create(CreateMentorDto dto)
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

            return mentor;
        }

        public async Task<Mentor?> Update(int id, UpdateMentorDto dto, int currentUserId, string role)
        {
            var mentor = await _context.Mentor.FindAsync(id);

            if (mentor == null)
                return null;

            if (role != "Admin")
            {
                if (role == "Mentor" && mentor.Id != currentUserId)
                    throw new UnauthorizedAccessException("Access denied");
            }

            mentor.FirstName = dto.FirstName;
            mentor.LastName = dto.LastName;
            mentor.Email = dto.Email;
            mentor.Expertise = dto.Expertise;
            mentor.Status = dto.Status;
            mentor.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return mentor;
        }

        public async Task<bool> Delete(int id, int currentUserId, string role)
        {
            var mentor = await _context.Mentor.FindAsync(id);

            if (mentor == null)
                return false;

            if (role != "Admin")
                throw new UnauthorizedAccessException("Only Admin can delete mentors");

            _context.Mentor.Remove(mentor);
            await _context.SaveChangesAsync();

            return true;
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
}
