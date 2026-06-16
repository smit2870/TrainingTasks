using taskmanagement.Models.DTOs.Mentor;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Common;

namespace taskmanagement.Services
{
    public interface IMentorService
    {
        Task<PagedResponse<Mentor>> GetAll(string? search, MentorStatus? status, int pageNumber, int pageSize);
        Task<Mentor?> GetById(int id, int currentUserId, string role);

        Task<Mentor> Create(CreateMentorDto dto);
        Task<Mentor?> Update(int id, UpdateMentorDto dto, int currentUserId, string role);

        Task<bool> Delete(int id, int currentUserId, string role);

        MentorResponseDto MapToDto(Mentor mentor);
    }
}

