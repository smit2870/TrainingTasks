using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using taskmanagement.Models.DTOs.User;

    public class UserValidator
    {
        private readonly AppDbContext _context;

        public UserValidator(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task ValidateCreateAsync(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new Exception("Username is required");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new Exception("Email is required");

            if (!dto.Email.Contains("@"))
                throw new Exception("Invalid email format");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email already exists");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Username already exists");
        }

        public async Task ValidateUpdateAsync(int userId, UpdateUserDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (!dto.Email.Contains("@"))
                    throw new Exception("Invalid email format");

                if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
                    throw new Exception("Email already exists");
            }

            if (!string.IsNullOrWhiteSpace(dto.Username))
            {
                if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != userId))
                    throw new Exception("Username already exists");
            }
        }
    }
