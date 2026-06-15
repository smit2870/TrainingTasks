using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public int MentorId { get; set; }
        public required string Feedback { get; set; }
        public int? Score { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
        public DateTime ReviewedDate { get; set; }
    }
}