
namespace taskmanagement.Services
{
    
    public interface IAuthorizationService
    {
        Task CheckSubmissionAccess(int submissionId, int userId, string role);
        Task CheckFileAccess(int fileId, int userId, string role);
    }

}