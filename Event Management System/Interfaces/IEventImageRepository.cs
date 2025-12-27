using EventManagement.Models;

namespace Event_Management_System.Interfaces
{
    public interface IEventImageRepository
    {
        public Task AddeventImage(EventImage ev);
        public Task SaveChangesAsync();
    }
}
