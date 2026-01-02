using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.DTOs
{
    public class OrganizerApplicationCreateDTO
    {
        [Required(ErrorMessage = "Organization Name is required")]
        public string OrganizationName { get; set; }

        [Required(ErrorMessage = "Contact Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Contact Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Experience is required")]
        public string ExperienceDescription { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? WebsiteUrl { get; set; }
    }
}
