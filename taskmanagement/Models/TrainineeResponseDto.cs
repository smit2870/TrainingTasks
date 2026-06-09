using System.ComponentModel.DataAnnotations;

namespace taskmanagement.Models
{
    public class TraineeResponseDto
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }
    
        [EmailAddress]
        public required string Email { get; set; }

        public required string TechStack { get; set; }

        [Required]
        public TraineeStatus Status { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}