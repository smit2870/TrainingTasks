using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.TaskAssignment
{
    public class TaskAssignmentResponseDto
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public int MentorId { get; set; }
        public int LearningTaskId { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }

        public TaskAssignmentStatus Status { get; set; }
        public string? Remarks { get; set; }
    }
}