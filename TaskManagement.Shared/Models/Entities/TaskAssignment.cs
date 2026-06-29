using taskmanagement.Models.Enums;

namespace taskmanagement.Models.Entities
{
    public class TaskAssignment
    {
        public int Id { get; set; }
        public required int TraineeId { get; set; }
        public required int MentorId { get; set; }
        public required int LearningTaskId { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }

        public TaskAssignmentStatus Status { get; set; } = TaskAssignmentStatus.Assigned;

        public string? Remarks { get; set; }

        
        public Trainee? Trainee { get; set; }
        public Mentor? Mentor { get; set; }
        public LearningTask? LearningTask { get; set; }


    }
}