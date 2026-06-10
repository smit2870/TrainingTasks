using System.ComponentModel.DataAnnotations;

namespace taskmanagement.Models
{
    public class User{
        
        public int Id { get; set; } 
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}