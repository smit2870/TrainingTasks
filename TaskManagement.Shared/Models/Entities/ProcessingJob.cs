using taskmanagement.Models.Enums;

namespace taskmanagement.Models.Entities
{
    public class ProcessingJob
    {
        public int Id { get; set; }

        public Guid MessageId { get; set; }
        public Guid CorrelationId { get; set; }

        public int SubmissionId { get; set; }
        public Submission Submission { get; set; } = null!;
        public int FileId { get; set; }
        public SubmissionFile File { get; set; } = null;

        public JobStatus Status { get; set; } = JobStatus.Queued;

        public int Attempts { get; set; }
        public string? ErrorSummary { get; set; }

        public string? Result { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}