using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.Models.Entities
{
    public class PaymentReciept
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ReceiptNumber { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

        [Required]
        public int UserId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int NumberOfTickets { get; set; }

        [Required]
        public decimal TicketPrice { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        // Store PDF as byte array
        public byte[]? ReceiptPdf { get; set; }

        // Navigation properties (optional)
        public User? User { get; set; }
        public Event? Event { get; set; }
    }
}
