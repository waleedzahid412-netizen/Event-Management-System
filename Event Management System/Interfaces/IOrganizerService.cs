using Event_Management_System.DTOs;
using EventManagement.Models;

namespace Event_Management_System.Interfaces
{
    public interface IOrganizerService
    {
        Task<OrganizerDashboardDTO> GetDashboardAsync(int organizerId);
        Task CreateEventAsync(CreateEventDTO dto, int organizerId);
        Task<List<EventManagement.Models.Event>> GetEventsByOrganizerAsync(int organizerId,string status);
        Task<List<EventCategory>> GetAllEventCategoryAsync();

        Task<OrganizerEventDetailsDTO>  GetEventDetailsAsync(int eventId);
        Task<List<EventParticipantDTO>> GetEventParticipantsAsync(int eventId);
    }
}
