using Event_Management_System.Models.Entities;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> Getuserwithrolebyname(string Fullname);
        Task<bool> UserExistAsync(string Fullname);
        Task<Role?> findroleobjectofcustomer();

        Task SaveChangesAsync();

        Task AddUserRoleAsync(UserRole user);

        Task AddUserAsync(User user);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> UserEmailExistAsync(string Email);

     }
}
