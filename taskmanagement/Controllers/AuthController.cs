using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using Microsoft.AspNetCore.Authorization;
using taskmanagement.Models.DTOs.User;


namespace taskmanagement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly IConfiguration _config;
        private readonly TokenProviderService _tokenProvider;
        public AuthController(AppDbContext appDbContext, PasswordService passwordService, IConfiguration configuration, TokenProviderService tokenProvider)
        {
            _context = appDbContext;
            _passwordService = passwordService;
            _config = configuration;
            _tokenProvider = tokenProvider;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

            if(user == null || !_passwordService.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username and password");
            }

            var token = _tokenProvider.GenerateJwtToken(user);
            
            return Ok(new
            {
                token,
                expiresIn = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60") * 60,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    role = user.Role.ToString()
                }
            });
        }

    }
}