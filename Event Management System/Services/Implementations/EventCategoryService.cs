using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;

namespace Event_Management_System.Services.Implementations
{
    public class EventCategoryService : IEventCategoryService
    {
        private readonly IEventCategoryRepository _eventCategoryrepository;
        public EventCategoryService(IEventCategoryRepository _eventrepo) {
            _eventCategoryrepository = _eventrepo;
        }
        public Task<List<EventCategory>> GetAllEventCategoryAsync()
        {
            return _eventCategoryrepository.GetEventCategoryAsync();
        }
    }
}
