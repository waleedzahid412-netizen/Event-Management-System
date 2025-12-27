using Event_Management_System.Models;
using EventManagement.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }
        public int OrganizerId { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Upcoming";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CoverImageUrl { get; set; }

        // Navigation
        public EventCategory Category { get; set; }
        public User Organizer { get; set; }

        // Remove '?' and initialize with '= new List<Registration>();'
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<Notification> Notifications { get; set; } =new List<Notification>();
        public ICollection<EventImage> EventImages { get; set; }=new List<EventImage>();
    }
}
