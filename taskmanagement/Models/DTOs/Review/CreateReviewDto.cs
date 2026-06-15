using taskmanagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace taskmanagement.Models.DTOs.Review
{
    public class CreateReviewDto
    {
        public int SubmissionId { get; set; }
        public int MentorId { get; set; }
        public required string Feedback { get; set; }

        [Range(1, 10, ErrorMessage = "Score must be between 1 to 10")]
        public int? Score { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
    }
}