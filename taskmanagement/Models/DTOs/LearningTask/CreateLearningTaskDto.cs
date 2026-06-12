using System.ComponentModel.DataAnnotations;
using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.LearningTask
{
    public class CreateLearningTaskDto
    {
        [StringLength(100, MinimumLength = 3)]
        public required string Title { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public required string Description { get; set; }
        public required string ExpectedTechStack { get; set; }
        public DateTime DueDate { get; set; }
        public LearningTaskStatus Status { get; set; }

    }
}