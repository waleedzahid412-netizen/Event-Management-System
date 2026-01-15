using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Repositories.Implementations
{

    public class EventCategoryRepository :IEventCategoryRepository
    {
        public readonly ApplicationDbContext _context;
        public EventCategoryRepository(ApplicationDbContext context) { 
        _context = context;
        }
        public async Task<List<EventCategory>> GetEventCategoryAsync()
        {
            return await _context.EventCategories.ToListAsync();
        }
    }
}
