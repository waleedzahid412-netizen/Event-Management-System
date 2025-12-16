using EventManagement.Models;

using System;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models
{
    public class Registration
    {
        public int RegistrationId { get; set; }

        // Foreign Keys
        public int EventId { get; set; }
        public int UserId { get; set; }

        public DateTime RegisteredOn { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        [MaxLength(50)]
        public string TicketNumber { get; set; }

        public bool CheckInStatus { get; set; } = false;

        // Navigation
        public Event Event { get; set; }
        public User User { get; set; }
        public Payment? Payment { get; set; }
    }
}
