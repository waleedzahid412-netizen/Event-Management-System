using Event_Management_System.Interfaces;
using EventManagement.Data;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Repository
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

        public async Task<User?> Getuserwithrolebyname(string Fullname) {
            return  await _context.Users.Include(u => u.UserRoles)
                 .ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.FullName.ToLower().Equals(Fullname));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistAsync(string Fullname)
        {
            return await _context.Users.AnyAsync(u => u.FullName.ToLower().Equals(Fullname.ToLower()));
        }
    }
}
