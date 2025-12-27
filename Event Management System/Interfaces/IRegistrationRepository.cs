using EventManagement.Models;

namespace Event_Management_System.Interfaces
{
    public interface IRegistrationRepository
    {
        Task AddRegistrationAsync(Registration registration);
        Task SaveChangesAsync();
        Task<bool> IsUserAlreadyRegisteredAsync(int userId, int eventId);
    }
}
