using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.Models.Entities
{
    public class EventReview
    {
        [Key]
        public int EventReviewId { get; set; }

        // Foreign keys
        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }

        // Rating: 1 to 5 stars
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        // Optional comment
        [MaxLength(500)]
        public string? Comment { get; set; }

        // When the review was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Event Event { get; set; }
        public User User { get; set; }
    }
}
