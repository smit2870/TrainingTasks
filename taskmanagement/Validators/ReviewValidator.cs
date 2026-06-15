using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;
using taskmanagement.Models.DTOs.Review;

public class ReviewValidator
{
    private readonly AppDbContext _context;

    public ReviewValidator(AppDbContext context)
    {
        _context = context;
    }

    public async Task ValidateCreateAsync(CreateReviewDto dto)
    {
        if (!await _context.Mentor.AnyAsync(x => x.Id == dto.MentorId))
            throw new Exception("Invalid MentorId");

        if (!await _context.Submission.AnyAsync(x => x.Id == dto.SubmissionId))
            throw new Exception("Invalid SubmissionId");
    }

}