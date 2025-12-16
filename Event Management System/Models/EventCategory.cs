using EventManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models
{
    public class EventCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, MaxLength(100)]
        public string CategoryName { get; set; }

        // Navigation
        public ICollection<Event>? Events { get; set; }
    }
}
