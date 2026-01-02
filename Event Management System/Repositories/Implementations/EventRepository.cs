using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using EventManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Event_Management_System.Repositories.Implementations
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

        public async Task<List<Event>> GetEventsByAttendeeId(int userid)
        {
            return await _context.Events
         .Where(e => e.Registrations.Any(r => r.UserId == userid))
         .ToListAsync();
        }

        public async Task<List<Event>> GetEventsByAttendeeIdAndCategoryId(int userid, int categoryid)
        {
            return await _context.Events
                .Where(e => e.Registrations
                .Any(r => r.UserId == userid) && e.CategoryId == categoryid)
                .ToListAsync();
        }

        public async Task<int> CountofEventAttendedByUserid(int id)
        {
            return await _context.Events.CountAsync(e => e.Registrations.Any(r => r.UserId == id && e.EndDate < DateTime.UtcNow));    
        }

        public async Task<int> CountofUpcomingEventByUserid(int id)
        {
            return await _context.Events.CountAsync(e => e.Registrations.Any(r => r.UserId == id && e.StartDate > DateTime.UtcNow));
        }

        public async Task<int> GetAvailableSeatsAsync(int eventid)
        {
            return await _context.Events
                .Where(e => e.EventId == eventid)
                .Select(e => e.AvailableSeats)
                .FirstOrDefaultAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            await transaction.CommitAsync();
        }

        public async Task RollBackTransactionAsync(IDbContextTransaction transaction)
        {
            await transaction.RollbackAsync();
        }
    }
}
