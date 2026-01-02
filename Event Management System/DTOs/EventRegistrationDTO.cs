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

        [Range(1, 10, ErrorMessage = "You can register between 1 and 10 tickets.")]
        public int NumberOfTickets { get; set; } = 1;
        [MaxLength(50)] 
        public List<string>? TicketNumbers { get; set; }
    }
}
