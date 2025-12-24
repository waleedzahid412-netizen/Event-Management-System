using Event_Management_System.Interfaces;
using EventManagement.Configuration;
using EventManagement.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Event_Management_System.Services
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
            new System.Security.Claims.Claim(ClaimTypes.Name,user.FullName),
            new System.Security.Claims.Claim(ClaimTypes.Role,assignedrole ?? "Customer")
            };
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwt.Key));
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
