using System.ComponentModel.DataAnnotations;

namespace taskmanagement.Models{
    public class Trainee{
        
        public Trainee()
        {
                FirstName = string.Empty;
                LastName  = string.Empty;
                Email     = string.Empty;
                TechStack = string.Empty;
        }

        public int Id { get; set; } 
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string TechStack { get; set; }
        public TraineeStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}