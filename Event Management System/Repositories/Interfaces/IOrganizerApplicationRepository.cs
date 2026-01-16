using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IOrganizerApplicationRepository
    {
        Task<bool> CheckIfOrganizerApplicationExistAsync(int userid);
        Task AddApplicationAsync(OrganizerApplication obj);
        Task SaveChangesAsync();
        public Task<OrganizerApplication?> GetOrganizerApplicationByIdAsync(int applicationid);
        public Task<bool> checkIfApplicationExistButPaymentPending(int userid);
        public Task<int> GetApplicationIdOfUser(int userid);

        public Task<int> TotalOrganizerPendingRequestAsync();

        public Task<List<OrganizerApplication>> GetRejectedOrganizerApplications();
        public Task<List<OrganizerApplication>> GetApprovedOrganizerApplications();

        public Task<List<OrganizerApplication>> GetPendingOrganizerApplications();
        public Task<OrganizerApplication?> ApproveApplication(int Applicationid);
    }
}
