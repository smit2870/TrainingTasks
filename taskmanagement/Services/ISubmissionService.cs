using taskmanagement.Models.DTOs.Submission;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;

namespace taskmanagement.Services
{
    public interface ISubmissionService
    {
        
        Task<Submission> CreateSubmission(CreateSubmissionDto dto);
        Task<IEnumerable<Submission>> GetAll();
        Task<SubmissionResponseDto?> GetById(int id, int userId, string role);
        SubmissionResponseDto MapToDto(Submission submission);

        Task<SubmissionFile> UploadFile(int submissionId, IFormFile file, string userName);
        Task<(Stream stream, SubmissionFile file)> DownloadFile(int fileId, int userId, string role);
        Task DeleteFile(int fileId, int userId, string role);
    }
}