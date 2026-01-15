using Event_Management_System.Models.Entities;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IEventCategoryRepository
    {

        public Task<List<EventCategory>> GetEventCategoryAsync();
    }
}
