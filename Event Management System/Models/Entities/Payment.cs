using System;


namespace Event_Management_System.Models.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }

        // Foreign Key
        public int RegistrationId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }
        public string Status { get; set; } = "Pending";

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public Registration Registration { get; set; }
    }
}
