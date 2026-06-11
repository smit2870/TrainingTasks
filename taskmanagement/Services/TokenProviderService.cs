using Microsoft.EntityFrameworkCore;
using taskmanagement.Models.Entities;
using taskmanagement.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace taskmanagement.Services
{
    public class TokenProviderService
    {
        private readonly IConfiguration _config;

        public TokenProviderService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:key"]);

            var claims = new[]
            {
                new Claim("UserId",user.Id.ToString()),
                new Claim("username",user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                
            var creds = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256);

                
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
