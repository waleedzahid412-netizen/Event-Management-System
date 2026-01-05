using Event_Management_System.Models.Entities;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IEventReviewRepository
    {
        public Task AddEventReview(EventReview review);
        public Task<bool> CheckIfUserReviewExist(int Eventid,int userid);
        public Task SaveChangesAsync();
    }
}
