using Event_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.DTOs
{
    public class OrganizerApplicationReviewDTO
    {
        [Required]
        public int OrganizerApplicationId { get; set; }

        [Required(ErrorMessage = "Please select status")]
        public ApplicationStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string AdminComments { get; set; }
    }
}
