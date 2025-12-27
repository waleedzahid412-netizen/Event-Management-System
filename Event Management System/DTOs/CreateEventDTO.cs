using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.DTOs
{
    public class CreateEventDTO
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        // Images
        [Required]
        public IFormFile CoverImage { get; set; }

        public List<IFormFile>? VenueImages { get; set; }
    }
}
