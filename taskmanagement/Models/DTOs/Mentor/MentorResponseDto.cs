using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.Mentor
{
    public class MentorResponseDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Expertise { get; set; }
        public MentorStatus Status { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}