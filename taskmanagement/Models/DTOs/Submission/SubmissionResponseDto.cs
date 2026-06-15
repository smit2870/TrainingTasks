using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.Submission
{
    public class SubmissionResponseDto
    {
        public int Id { get; set; }
        public int TaskAssignmentId { get; set; }
        public string SubmissionUrl { get; set; }
        public string Notes { get; set; }
        public DateTime SubmittedDate { get; set; }
        public SubmissionStatus Status { get; set; }
    }

}