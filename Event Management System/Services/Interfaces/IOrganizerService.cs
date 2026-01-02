using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;

namespace Event_Management_System.Services.Interfaces
{
    public interface IOrganizerService
    {
        Task<OrganizerDashboardDTO> GetDashboardAsync(int organizerId);
        Task CreateEventAsync(CreateEventDTO dto, int organizerId);
        Task<List<Event>> GetEventsByOrganizerAsync(int organizerId,string status);
        Task<List<EventCategory>> GetAllEventCategoryAsync();

        Task<OrganizerEventDetailsDTO>  GetEventDetailsAsync(int eventId);
        Task<List<EventParticipantDTO>> GetEventParticipantsAsync(int eventId);
    }
}
