using System.ComponentModel.DataAnnotations;
using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.Common
{
    public class ProcessingJobDto
    {
        public int Id { get; set; }

        public Guid MessageId { get; set; }
        public Guid CorrelationId { get; set; }
        public int SubmissionId { get; set; }
        public int FileId { get; set; }

        public JobStatus Status { get; set; }
        public int Attempts { get; set; }
        public string? ErrorSummary { get; set; }

        public string? Result { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

