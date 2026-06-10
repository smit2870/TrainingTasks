
namespace taskmanagement.Models
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}