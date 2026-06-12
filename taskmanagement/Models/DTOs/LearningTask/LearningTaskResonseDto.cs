using System.ComponentModel.DataAnnotations;
using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.LearningTask
{
    public class LearningTaskResponseDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string ExpectedTechStack { get; set; }
        public DateTime? DueDate { get; set; }
        public LearningTaskStatus Status { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}