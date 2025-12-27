using EventManagement.Models;

namespace EventManagement.Models
{
    public class EventImage
    {
        public int EventImageId { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public string ImageUrl { get; set; }
    }
}
