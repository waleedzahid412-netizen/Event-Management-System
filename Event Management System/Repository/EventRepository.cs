using Event_Management_System.Interfaces;
using EventManagement.Data;
using EventManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Event_Management_System.Repository
{
    public class EventRepository : IEventRepository
    {
        ApplicationDbContext _context;
        public EventRepository(ApplicationDbContext context) {
        _context= context;
                }

        public async Task AddEventAsync(Event ev)
        {
            await _context.Events.AddAsync(ev);
        }

        public  async Task<List<Event>> GetAllEventsWithCategoryAsync
()
        {
            return await _context.Events.Include(e => e.Category).ToListAsync();     
        }

        public async Task<List<Event>> GetUpcomingEventByOrganizerIdAsync(int organizerId)
        {
            var todaysdate = DateTime.UtcNow;
            return await _context.Events.Include(e => e.Category)
                .Where(er => er.OrganizerId == organizerId && er.StartDate> todaysdate)
                .ToListAsync();
        }
        public async Task<List<Event>> GetCompletedEventByOrganizerIdAsync(int organizerid) {
            var todaysdate = DateTime.UtcNow;
            return await _context.Events.Include(e => e.Category)
                .Where(er => er.OrganizerId == organizerid && er.EndDate < todaysdate).ToListAsync(); ;
        }

        public async Task<List<EventCategory>> GetEventCategoryAsync()
        {
            return await _context.EventCategories.ToListAsync();
        }
        [Authorize("Organizer")]

        public async Task<Event?> GetEventDetailsByIdAsync(int eventid)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.EventImages)
                .Include(e => e.Registrations)
                  .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(e => e.EventId == eventid);
        }
        public async Task<Event?> GetEventDetailsForCustomerByIdAsync(int eventid)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.EventImages)
                .FirstOrDefaultAsync(e => e.EventId == eventid);
        }


        public async Task<List<Registration>> GetEventParticipantsbyEventIdAsync(int eventid)
        {
            return await _context.Registrations
               .Include(r => r.User)
               .Where(r => r.EventId == eventid)
               .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
