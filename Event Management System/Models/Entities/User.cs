using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }

        [Required, MaxLength(150)]
        public string FullName { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public string Password{get; set; }

        public int age { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<OrganizerApplication> OrganizerApplications { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
