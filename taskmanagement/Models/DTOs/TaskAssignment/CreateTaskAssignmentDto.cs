
namespace taskmanagement.Models.DTOs.TaskAssignment
{
    public class CreateTaskAssignmentDto
    {
        public int TraineeId { get; set; }
        public int MentorId { get; set; }
        public int LearningTaskId { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }

        public string? Remarks { get; set; }
    }
}

