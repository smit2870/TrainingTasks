using taskmanagement.Models.Enums;

namespace taskmanagement.Models.Entities
{
    public class Submission
    {
        public int Id { get; set; }
    
        public int TaskAssignmentId { get; set; } 

        public required string SubmissionUrl { get; set; } 

        public required string Notes { get; set; } 

        public DateTime SubmittedDate { get; set; }

        public SubmissionStatus Status { get; set; }  

        public TaskAssignment? TaskAssignment { get; set; }
        public ICollection<ProcessingJob> ProcessingJobs { get; set; }
    }

}