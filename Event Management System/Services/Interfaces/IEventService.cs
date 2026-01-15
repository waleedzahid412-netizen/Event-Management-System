using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Models.Enums;

namespace Event_Management_System.Services.Interfaces
{
    public interface IEventService
    {
        public  Task<bool> UpdateEventStatus(EventStatus status, int eventid);
        Task CreateEventAsync(CreateEventDTO dto, int organizerId);
        Task<bool> SendCancelEventEmailToTheParticipantsAsync(int eventid, string cancelreason);
        Task<OrganizerEventDetailsDTO> GetOrganizerEventDetailsAsync(int eventId);
        Task<List<Event>> GetEventsByOrganizerAsync(int organizerId, string status);
        public  Task<CustomerEventDetailsDTO> GetCustomerEventDetailsAsync(int id);
        public Task RegisterForEventAsync(int userId, int eventId, int nooftickets);
        public Task<List<Registration>> GetRegistrationsForUserEventAsync(int userId, int eventId);
        public  Task<List<Event>> BrowseEventAsync(int? categoryid, string status);
        public  Task<List<Event>> GetEventByAttendeeId(int userid, int? categoryid, bool showrecommended);


    }
}
