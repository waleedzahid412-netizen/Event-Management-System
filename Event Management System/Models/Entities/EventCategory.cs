using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.Models.Entities
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
