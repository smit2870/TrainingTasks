using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.Submission
{
    public class CreateSubmissionDto
    {
        public int TaskAssignmentId { get; set; }
        public required string SubmissionUrl { get; set; }
        public required string Notes { get; set; }
        public SubmissionStatus Status { get; set; }
    }
}