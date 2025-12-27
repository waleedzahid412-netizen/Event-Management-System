using EventManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management_System.Interfaces
{
    public interface IEventRepository
    {
        public Task<List<Event>> GetAllEventsWithCategoryAsync();
        public Task  AddEventAsync(Event ev);
        public Task SaveChangesAsync();
        public Task<List<Event>> GetUpcomingEventByOrganizerIdAsync(int organizerId);

        public Task<List<EventCategory>> GetEventCategoryAsync();
        public Task<Event?> GetEventDetailsByIdAsync(int eventid);

        public Task<List<Registration>> GetEventParticipantsbyEventIdAsync(int eventid);
        public Task<List<Event>> GetCompletedEventByOrganizerIdAsync(int organizerid);
        public Task<Event?> GetEventDetailsForCustomerByIdAsync(int eventid);
    }
}
