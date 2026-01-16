using Event_Management_System.DTOs;
using Event_Management_System.Models;
using Event_Management_System.Models.Entities;
using Event_Management_System.ViewModels;

namespace Event_Management_System.Services.Interfaces
{
    public interface IOrganizerApplicationService
    {

        public Task<OrganizerApplicationCreateDTO> GetOrganizerApplicationByIdAsync(int applicationid);


        Task<int> SubmitOrganizerApplication(OrganizerApplicationCreateDTO dto, int Userid);

        public Task<bool> checkIfApplicationExistButPaymentPending(int userid);
        public Task<int> GetApplicationIdOfUser(int userid);
        Task<List<OrganizerApplicationListVM>> GetApplicationsByStatus(ApplicationStatus status);
        public  Task<OrganizerApplicationsDetailVM?> GetApplicationDetailsAsync(int id);
        public Task<bool> ApproveApplicationAsync(int applicationId, int userid, string email, int adminId);
        public  Task<bool> RejectApplicationAsync(int applicationId, string useremail, int adminId, string comments);


    }
}
