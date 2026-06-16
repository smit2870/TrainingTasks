using taskmanagement.Models.DTOs.Common;
using taskmanagement.Models.DTOs.User;

namespace taskmanagement.Services
{
    public interface IUserService
    {
        Task<PagedResponse<UserResponseDto>> GetAll(string? search, int pageNumber, int pageSize);
        Task<UserResponseDto?> GetById(int id);
        Task<UserResponseDto> Create(CreateUserDto dto);
        Task<UserResponseDto?> Update(int id, UpdateUserDto dto);
        Task<bool> Delete(int id);
    }
}
