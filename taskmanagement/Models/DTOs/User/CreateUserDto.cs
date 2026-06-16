
using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.User
{
    public class CreateUserDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required UserRole Role { get; set; }
    }
}
