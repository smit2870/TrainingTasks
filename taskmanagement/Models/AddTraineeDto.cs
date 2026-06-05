using System.ComponentModel.DataAnnotations;

namespace taskmanagement.Models
{
    public class AddTraineeDto
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name must contain only letters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name must contain only letters")]
        public string LastName { get; set; }
    
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string TechStack { get; set; }

        [Required]
        [AllowedValues("Available","Offline","Busy","Do Not Disturb",ErrorMessage ="Status should be Available , Offline , Busy or Do Not Disturb")]
        public string Status { get; set; }

    }
}