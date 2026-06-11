using System.ComponentModel.DataAnnotations;
using taskmanagement.Models.Enums;

namespace taskmanagement.Models.Entities
{
    public class Mentor{
        
        public Mentor()
        {
                FirstName = string.Empty;
                LastName  = string.Empty;
                Email     = string.Empty;
                Expertise = string.Empty;
        }

        public int Id { get; set; } 
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Expertise { get; set; }
        public MentorStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}