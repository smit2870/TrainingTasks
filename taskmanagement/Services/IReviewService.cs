using taskmanagement.Models.Entities;
using taskmanagement.Models.DTOs.Review;

namespace taskmanagement.Services
{
    public interface IRerviewService
    {
        Task<Review> Create(CreateReviewDto dto);
        Task<IEnumerable<Review>> GetAll();
        Task<Review?> GetById(int id, int currentUserId, string role);
        ReviewResponseDto MapToDto(Review review);

    }
}