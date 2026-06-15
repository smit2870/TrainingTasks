using Microsoft.EntityFrameworkCore;
using taskmanagement.Models.Entities;
using taskmanagement.Models.DTOs.Review;
using taskmanagement.Models.Enums;
using taskmanagement.Data;

namespace taskmanagement.Services
{
    public class ReviewService : IRerviewService
    {
        public readonly AppDbContext _context;
        public readonly ILogger<ReviewService> _logger;
        public readonly ReviewValidator _validator;

        public ReviewService(AppDbContext context, ILogger<ReviewService> logger, ReviewValidator validator)
        {
            _context = context;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Review> Create(CreateReviewDto dto)
        {
            try
            {
                await _validator.ValidateCreateAsync(dto);

                var review = new Review
                {
                    SubmissionId = dto.SubmissionId,
                    MentorId = dto.MentorId,
                    Feedback = dto.Feedback,
                    Score = dto.Score,
                    ReviewStatus = dto.ReviewStatus,
                    ReviewedDate = DateTime.UtcNow
                };

                await _context.Review.AddAsync(review);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Review created successfully Id={Id}", review.Id);

                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error while submitting Review");
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetAll()
        {
            try
            {
                return await _context.Review.ToListAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e,"Error while getting Review data.");
                throw;
            }
        }

        public async Task<Review?> GetById(int id)
        {
            try
            {
                var review = await _context.Review.FindAsync(id);
                return review;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e , "Error while fetching Review Id={Id}",id);
                throw;
            }
        }

        public ReviewResponseDto MapToDto(Review review)
        {
            return new ReviewResponseDto
            {
                Id = review.Id,
                SubmissionId = review.SubmissionId,
                MentorId = review.MentorId,
                Feedback = review.Feedback,
                Score = review.Score,
                ReviewStatus = review.ReviewStatus,
                ReviewedDate = review.ReviewedDate
            };
        }
    }
}