using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.DTOs
{
    public class EventReviewCreateDTO
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
