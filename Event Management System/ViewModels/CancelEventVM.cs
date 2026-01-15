using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.ViewModels
{

        public class CancelEventVM
        {
            // Event info (display only)
            public int EventId { get; set; }
            public string Title { get; set; }
            public DateTime EventDate { get; set; }
            public string Category { get; set; }
            public string Venue { get; set; }
            public int TotalParticipants { get; set; }
            public string Description { get; set; }
            public string? Status { get; set; }

            // Organizer input
            [Required(ErrorMessage = "Please provide a reason for cancelling the event")]
            [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
            public string CancelReason { get; set; }
        

    }
}
