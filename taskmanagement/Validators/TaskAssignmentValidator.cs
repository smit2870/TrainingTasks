using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.TaskAssignment;

public class TaskAssignmentValidator
{
    private readonly AppDbContext _context;

    public TaskAssignmentValidator(AppDbContext context)
    {
        _context = context;
    }

    
    public async Task ValidateCreateAsync(CreateTaskAssignmentDto dto)
    {
        if (!await _context.Trainees.AnyAsync(x => x.Id == dto.TraineeId))
            throw new Exception("Invalid TraineeId");

        if (!await _context.Mentor.AnyAsync(x => x.Id == dto.MentorId))
            throw new Exception("Invalid MentorId");

        if (!await _context.LearningTask.AnyAsync(x => x.Id == dto.LearningTaskId))
            throw new Exception("Invalid LearningTaskId");

        if (dto.DueDate < dto.AssignedDate)
            throw new Exception("DueDate cannot be before AssignedDate");
    }


    public void ValidateStatusUpdate(TaskAssignment assignment, UpdateTaskAssignmentDto dto)
    {
        if (assignment.Status == TaskAssignmentStatus.Completed)
            throw new Exception("Task already completed");

        if (assignment.Status == TaskAssignmentStatus.Assigned &&
            dto.Status == TaskAssignmentStatus.Completed)
        {
            throw new Exception("Cannot move directly to Completed");
        }
    }
}