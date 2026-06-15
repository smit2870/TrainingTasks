using taskmanagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace taskmanagement.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public int MentorId { get; set; }
        public required string Feedback { get; set; }

        [Range(1,10)]
        public int? Score { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
        public DateTime ReviewedDate { get; set; }
                
        public Submission? Submission { get; set; }
        public Mentor? Mentor { get; set; }

    }
}