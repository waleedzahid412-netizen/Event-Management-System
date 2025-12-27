using Event_Management_System.DTOs;
using EventManagement.Models;

namespace Event_Management_System.Interfaces
{
    public interface IAttendeeService
    {
        Task<List<Event>> GetAllEventsAsync();
        public Task<CustomerEventDetailsDTO> GetEventDetailsAsync(int eventId);
        Task RegisterForEventAsync(int userId, int eventId);

        Task<List<EventCategory>> GetAllCategoriesAsync();
    }
}
