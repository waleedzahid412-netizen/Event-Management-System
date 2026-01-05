using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.Models.Entities
{
    public enum PaymentStatus
    {
        Pending=0,
        Succeeded=1,
        Failed=2
    }
    public class OrganizerPayment
    {
        [Key]
        public int PaymentId { get; set; }

        public int OrganizerApplicationId { get; set; }
        public OrganizerApplication OrganizerApplication { get; set; }

        public decimal Amount { get; set; }

        [StringLength(100)]
        public string StripePaymentIntentId { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
