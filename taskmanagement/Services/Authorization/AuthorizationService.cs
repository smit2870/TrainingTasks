using taskmanagement.Data;

namespace taskmanagement.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly AppDbContext _context;

        public AuthorizationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CheckSubmissionAccess(int submissionId, int userId, string role)
        {
            var submission = await _context.Submission.FindAsync(submissionId);
            if (submission == null)
                throw new Exception("Submission not found");

            var assignment = await _context.TaskAssignment.FindAsync(submission.TaskAssignmentId);
            if (assignment == null)
                throw new Exception("Assignment not found");
            
            if (role == "Mentor" && userId != assignment.MentorId)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            if (role == "Trainee" && userId != assignment.TraineeId)
            {
                throw new UnauthorizedAccessException("Access denied");
            }
        }

        public async Task CheckFileAccess(int fileId, int userId, string role)
        {
            var file = await _context.SubmissionFiles.FindAsync(fileId);
            if (file == null)
                throw new Exception("File not found");

            await CheckSubmissionAccess(file.SubmissionId, userId, role);
        }
    }
}