using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Models.Enums;
using Event_Management_System.ViewModels;

namespace Event_Management_System.Services.Interfaces
{
    public interface IOrganizerService
    {
        Task<OrganizerDashboardDTO> GetDashboardAsync(int organizerId);



        Task<List<EventParticipantDTO>> GetEventParticipantsAsync(int eventId);
        Task<OrganizerDashboardVM> GetAnalyticsDataAsync(int userid, DateFilterType filter);



    }
}
