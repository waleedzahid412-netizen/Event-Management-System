using Event_Management_System.Models.Entities;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IOrganizerApplicationRepository
    {
        Task<bool> CheckIfOrganizerApplicationExistAsync(int userid);
        Task AddApplicationAsync(OrganizerApplication obj);
        Task SaveChangesAsync();
    }
}
