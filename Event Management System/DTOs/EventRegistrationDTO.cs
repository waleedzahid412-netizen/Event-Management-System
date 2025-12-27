using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.DTOs
{
    public class EventRegistrationDTO
    {
        [Required]
        public int EventId { get; set; }

        // If needed, you can add payment info or other fields
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        [MaxLength(50)] 
        public string? TicketNumber { get; set; }
    }
}
