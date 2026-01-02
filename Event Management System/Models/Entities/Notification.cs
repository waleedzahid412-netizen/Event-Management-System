using System;
using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.Models.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int EventId { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
        public Event Event { get; set; }
    }
}
