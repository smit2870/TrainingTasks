using System.ComponentModel.DataAnnotations;

namespace taskmanagement.Models{
    public class Trainee{
        public int? Id { get; set; } 

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string TechStack { get; set; }

        [Required]
        public TraineeStatus? Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}