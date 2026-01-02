using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Repositories.Implementations
{
    public class UserRepository :IUserRepository
    {
        ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) {
            _context = context;

        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task AddUserRoleAsync(UserRole userrole)
        {
            await _context.UserRoles.AddAsync(userrole);
        }

        public  async Task<Role?> findroleobjectofcustomer()
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Customer");
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            return _context.Users.FirstAsync(u => u.UserId == id);
        }

        public async Task<User?> Getuserwithrolebyname(string email) {
            return  await _context.Users.Include(u => u.UserRoles)
                 .ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistAsync(string Fullname)
        {
            return await _context.Users.AnyAsync(u => u.FullName.ToLower().Equals(Fullname.ToLower()));
        }
        public async Task<bool> UserEmailExistAsync(string Email) { 
        return await _context.Users.AnyAsync(u => u.Email.ToLower().Equals(Email.ToLower()));
        }
        
    }
}
