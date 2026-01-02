using Event_Management_System.Models.Entities;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IEventImageRepository
    {
        public Task AddeventImage(EventImage ev);
        public Task SaveChangesAsync();
    }
}
