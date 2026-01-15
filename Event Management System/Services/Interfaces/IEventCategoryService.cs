using Event_Management_System.Models.Entities;

namespace Event_Management_System.Services.Interfaces
{
    public interface IEventCategoryService
    {
        Task<List<EventCategory>> GetAllEventCategoryAsync();
    }
}
