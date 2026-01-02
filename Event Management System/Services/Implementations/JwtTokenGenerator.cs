using Event_Management_System.Models.Entities;
using Event_Management_System.Services.Interfaces;
using EventManagement.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Event_Management_System.Services.Implementations
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwt;
        public JwtTokenGenerator(IOptions<JwtSettings> options)
        {
            _jwt = options.Value;
        }
        public string GenerateToken(User user, IEnumerable<string> role)
        {
            var assignedrole = role.FirstOrDefault();
            if (role.Contains("Admin"))
            {
                assignedrole = "Admin";
            }
            else if (role.Contains("Organizer"))
            {
                assignedrole = "Organizer";
            }
            else
            {
                assignedrole = "Customer";
            }

            var Claim = new[]
                {
            new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new System.Security.Claims.Claim(ClaimTypes.Name,user.FullName),
            new System.Security.Claims.Claim(ClaimTypes.Role,assignedrole ?? "Customer"),
            new System.Security.Claims.Claim(ClaimTypes.Email,user.Email)
            };
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF32.GetBytes(_jwt.Key));
            var creds = new Microsoft.IdentityModel
            .Tokens.
            SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: Claim,
                expires: System.DateTime.Now.AddMinutes(30),
                signingCredentials: creds
                );
            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

 
    }
}
