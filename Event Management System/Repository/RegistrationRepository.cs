using Event_Management_System.Interfaces;
using EventManagement.Data;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Repository
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly ApplicationDbContext _context;
        public RegistrationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddRegistrationAsync(Registration registration)
        {
            await _context.Registrations.AddAsync(registration);
        }

        public async Task<bool> IsUserAlreadyRegisteredAsync(int userId, int eventId)
        {
            return await _context.Registrations.AnyAsync(r => r.UserId == userId && r.EventId == eventId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
