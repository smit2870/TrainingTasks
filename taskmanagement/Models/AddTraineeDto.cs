using System.ComponentModel.DataAnnotations;

namespace taskmanagement.Models
{
    public class AddTraineeDto
    {
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name must contain only letters")]
        public required string FirstName { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name must contain only letters")]
        public required string LastName { get; set; }
    
        [EmailAddress]
        public required string Email { get; set; }
        public required string TechStack { get; set; }

        public TraineeStatus? Status { get; set; }

    }
}