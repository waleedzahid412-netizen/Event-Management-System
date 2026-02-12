using Event_Management_System.DTOs;

namespace Event_Management_System.ViewModels
{
    public class EventDetailsPageViewModel
    {
        public OrganizerEventDetailsDTO Event { get; set; }

        public List<OrganizerEventDetailsDTO> SimilarEvents { get; set; }
            = new();    
    }
}
