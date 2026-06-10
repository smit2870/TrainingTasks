using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanagement.Models;
using taskmanagement.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


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

        [HttpPost]
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

        // private string GenerateJwtToken(User user)
        // {
        //     var key = Encoding.UTF8.GetBytes(_config["Jwt:key"]);

        //     var claims = new[]
        //     {
        //         new Claim("UserId",user.Id.ToString()),
        //         new Claim("username",user.Username),
        //         new Claim(ClaimTypes.Role, user.Role.ToString())
        //     };

            
        //     var creds = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256);

            
        //     var token = new JwtSecurityToken(
        //         issuer: _config["Jwt:Issuer"],
        //         audience: _config["Jwt:Audience"],
        //         claims: claims,
        //         expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"])),
        //         signingCredentials: creds
        //     );

        //     return new JwtSecurityTokenHandler().WriteToken(token);
        // }

    }
}