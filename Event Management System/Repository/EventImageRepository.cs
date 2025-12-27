using Event_Management_System.Interfaces;
using EventManagement.Data;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Repository
{
    public class EventImageRepository : IEventImageRepository
        
    {
        public readonly ApplicationDbContext _context;
        public EventImageRepository(ApplicationDbContext context) { 
        _context = context;
        }
        public async Task AddeventImage(EventImage ev)
        {
            await _context.eventimage.AddAsync(ev);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
