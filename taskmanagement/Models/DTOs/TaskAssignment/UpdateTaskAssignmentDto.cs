using taskmanagement.Models.Enums;

namespace taskmanagement.Models.DTOs.TaskAssignment
{
    public class UpdateTaskAssignmentDto
    {
        public TaskAssignmentStatus Status { get; set; }
        public string? Remarks { get; set; }
    }
}
