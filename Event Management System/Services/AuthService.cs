using Azure;
using Event_Management_System.DTOs;
using Event_Management_System.Interfaces;
using EventManagement.Data;
using EventManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.Services
{
    public class AuthService : IAuthService
    {

        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly IUserRepository _userRepository;

        public AuthService(IJwtTokenGenerator gen,IUserRepository rep) {
            _jwtGenerator = gen;
            _userRepository = rep;
        }
        public  async Task<AuthResponseDTO> LoginAsync(LoginViewDTO dto)
        {

            var user = await _userRepository.Getuserwithrolebyname(dto.FullName);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                throw new Exception("Invalid password");
            }
            var role = user.UserRoles.Select(ur => ur.Role.RoleName);
            var token = _jwtGenerator.GenerateToken(user, role);
            return new AuthResponseDTO
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                Email = user.Email,
                roles = role

            };
        }

        public async Task RegisterAsync(RegisterViewDTO dto)
        {
            if (await _userRepository.UserExistAsync(dto.UserName)) {
                throw new Exception("User Already Exist ");
            }
            var user = new User
            {
                FullName = dto.UserName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                age = dto.age
            };
            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            var roles =await _userRepository.findroleobjectofcustomer();
            if (roles == null) { 
            throw new Exception("Role not found");
            }
            var userRole=new UserRole { 
                UserId=user.UserId,
                RoleId=roles.RoleId
            };
            await _userRepository.AddUserRoleAsync(userRole);
            await _userRepository.SaveChangesAsync();
        }
    }
}
