using taskmanagement.Models.Enums;

namespace taskmanagement.Models.Entities
{
    public class LearningTask{
        
        public LearningTask()
        {
                Title = string.Empty;
                Description = string.Empty;
                ExpectedTechStack = string.Empty;
        }

        public int Id { get; set; } 
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string ExpectedTechStack { get; set; }
        public DateTime DueDate { get; set; }
        public LearningTaskStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}