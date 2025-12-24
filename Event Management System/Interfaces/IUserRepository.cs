using EventManagement.Models;

namespace Event_Management_System.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> Getuserwithrolebyname(string Fullname);
        Task<bool> UserExistAsync(string Fullname);
        Task<Role?> findroleobjectofcustomer();

        Task SaveChangesAsync();

        Task AddUserRoleAsync(UserRole user);

        Task AddUserAsync(User user);
     }
}
