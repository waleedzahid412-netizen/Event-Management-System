using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.Models.Entities
{
    public class OrganizerApplication
    {
        [Key]
        public int OrganizerApplicationId { get; set; }

        // Applicant
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } // <-- navigation property

        [Required(ErrorMessage = "Organization Name is required")]
        [StringLength(200)]
        public string OrganizationName { get; set; }

        [Required(ErrorMessage = "Contact Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Contact Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Please describe your experience")]
        public string ExperienceDescription { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? WebsiteUrl { get; set; }

        // Status
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        // Admin review
        public int? ReviewedByAdminId { get; set; }
        public User ReviewedByAdmin { get; set; } // <-- navigation property
        [StringLength(500)]
        public string? AdminComments { get; set; }

        // Timestamps
        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedOn { get; set; }
    }
}
