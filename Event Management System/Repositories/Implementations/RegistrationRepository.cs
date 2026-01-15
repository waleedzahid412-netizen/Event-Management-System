using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using EventManagement.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe.Tax;

namespace Event_Management_System.Repositories.Implementations
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly ApplicationDbContext _context;
        public RegistrationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddRegistrationAsync(List<Models.Entities.Registration> registration)
        {
            await _context.Registrations.AddRangeAsync(registration);
        }

        public async  Task<List<Models.Entities.Registration>> GetRegistrationsForUserEventAsync(int userId, int eventId) { 
            return await _context.Registrations
                         .Where(r => r.UserId == userId && r.EventId == eventId)
                         .ToListAsync();
           
        }

        public async Task<bool> IsUserAlreadyRegisteredAsync(int userId, int eventId)
        {
            return await _context.Registrations.AnyAsync(r => r.UserId == userId && r.EventId == eventId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<string?> useremail(int userid) { 
        return await _context.Users
                .Where(u => u.UserId == userid)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Models.Entities.Registration>> GetEventParticipantsbyEventIdAsync(int eventid)
        {
            return await _context.Registrations
               .Include(r => r.User)
               .Where(r => r.EventId == eventid)
               .ToListAsync();
        }
    }
}
