using Event_Management_System.Models.Entities;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IRegistrationRepository
    {
        Task AddRegistrationAsync(List<Registration> registration);
        Task SaveChangesAsync();
        Task<bool> IsUserAlreadyRegisteredAsync(int userId, int eventId);
        Task<List<Registration>> GetRegistrationsForUserEventAsync(int userId, int eventId);
        public Task<string> useremail(int userid);

        public Task<List<Registration>> GetEventParticipantsbyEventIdAsync(int eventid);

    }
}
