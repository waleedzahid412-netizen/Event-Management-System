using Event_Management_System.Interfaces;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Repository
{
    public class OrganizerRepository : IOrganizerRepository
    {
        ApplicationDbContext _context;
        public OrganizerRepository(ApplicationDbContext context) { 
        _context = context;
        }

        public async Task<int> GetActiveEventsAsync(int organizerId)
        {
            var now = DateTime.UtcNow;

            return await _context.Events
                .Where(e => e.OrganizerId == organizerId
                         && e.StartDate <= now
                         && e.EndDate >= now
                         && e.Status == "Upcoming")
                .CountAsync();

        }

        public async Task<int> GetTotalEventsAsync(int organizerId)
        {
            return await _context.Events.Where(e => e.OrganizerId == organizerId).CountAsync();
        }

        public async Task<int> GetTotalRegistrationsAsync(int organizerId)
        {
            return await _context.Registrations.Include(r => r.Event)
                .Where(r => r.Event.OrganizerId == organizerId)
                .CountAsync();
        }

        public async Task<int> GetUpcomingEventsAsync(int organizerId)
        {
            var now = DateTime.UtcNow;

            return await _context.Events
                .Where(e => e.OrganizerId == organizerId
                         && e.StartDate > now
                         && e.Status == "Upcoming")
                .CountAsync();
        }
    }
}
