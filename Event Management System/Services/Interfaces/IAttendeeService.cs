using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;

namespace Event_Management_System.Services.Interfaces
{
    public interface IAttendeeService
    {
        Task<List<Event>> GetAllEventsAsync();
        public Task<CustomerEventDetailsDTO> GetEventDetailsAsync(int eventId);
        Task RegisterForEventAsync(int userId, int eventId,int nooftickets);

        Task<List<EventCategory>> GetAllCategoriesAsync();
        Task<List<Event>> GetEventByAttendeeId(int userid,int? categoryid,bool showrecommended);
        Task<UserProfileDTO> GetUserProfileAsync(int userId, string? role);

        Task<int> CountofEventAttendedByUserid(int id);
        Task<int> CountofUpcomingEventByUserid(int id);
        Task<List<Registration>> GetRegistrationsForUserEventAsync(int userId, int eventId);
        public Task<(PaymentReciept receipt, byte[] pdfBytes)> CreatePaymentReceiptAsync(int userId, CustomerEventDetailsDTO ev, List<Registration> registrations);
        Task SubmitOrganizerApplication(OrganizerApplicationCreateDTO dto,int Userid);
       
    }
}
