using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Repositories.Implementations
{
    public class EventReviewRepository : IEventReviewRepository
    {
        public readonly ApplicationDbContext _context;
        public EventReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public  async Task AddEventReview(EventReview review)
        {
            await _context.EventReviews.AddAsync(review);

        }

        public async Task<bool> CheckIfUserReviewExist(int Eventid, int userid)
        {
            return await _context.EventReviews.AnyAsync(e => e.EventId == Eventid && e.UserId == userid);
        }

        public async Task SaveChangesAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
