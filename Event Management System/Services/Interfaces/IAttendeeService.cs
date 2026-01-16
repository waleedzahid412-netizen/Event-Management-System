using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;

namespace Event_Management_System.Services.Interfaces
{
    public interface IAttendeeService
    {
        Task<List<Event>> GetAllEventsAsync();
        public Task<CustomerEventDetailsDTO> GetEventDetailsAsync(int eventId);


        Task<List<EventCategory>> GetAllCategoriesAsync();
        Task<List<Event>> GetEventByAttendeeId(int userid,int? categoryid,bool showrecommended);
        Task<UserProfileDTO> GetUserProfileAsync(int userId, string? role);

        Task<int> CountofEventAttendedByUserid(int id);
        Task<int> CountofUpcomingEventByUserid(int id);

      
        public Task<List<Event>> BrowseEventAsync(int? categoryid, string status);

        public Task AddEventReviewAsync(EventReviewCreateDTO dto,int userid);
        public Task<bool> CheckIfUserReviewExist(int Eventid,int userid);

        public Task<bool> checkIfEventExists(int eventid);



    }
}
