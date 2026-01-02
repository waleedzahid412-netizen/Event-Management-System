using Event_Management_System.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace Event_Management_System.Repositories.Interfaces
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
        public Task<List<Event>> GetEventsByAttendeeId(int userid);

        public Task<List<Event>> GetEventsByAttendeeIdAndCategoryId(int userid, int categoryid);
        public Task<int> CountofEventAttendedByUserid(int id);
        public Task<int>CountofUpcomingEventByUserid(int id);
        public Task<int> GetAvailableSeatsAsync(int eventid);
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync(IDbContextTransaction transaction);
        public Task RollBackTransactionAsync(IDbContextTransaction transaction);

    }
}
