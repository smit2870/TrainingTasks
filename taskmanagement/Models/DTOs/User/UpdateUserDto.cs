using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.User
{
    public class UpdateUserDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; } 
        public UserRole? Role { get; set; }
    }
}
