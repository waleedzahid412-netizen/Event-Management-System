using Event_Management_System.Models;

namespace Event_Management_System.ViewModels
{
    public class OrganizerApplicationsDetailVM
    {
        // Application
        public int OrganizerApplicationId { get; set; }
        public ApplicationStatus Status { get; set; }
        public bool IsPaymentCompleted { get; set; }

        // Applicant
        public int UserId { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }

        // Organization
        public string OrganizationName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ExperienceDescription { get; set; }
        public string? WebsiteUrl { get; set; }

        // Review info
        public string? ReviewedByAdminName { get; set; }
        public string? AdminComments { get; set; }
        public DateTime? ReviewedOn { get; set; }

        // Timestamps
        public DateTime AppliedOn { get; set; }

        // UI helpers
        public bool CanApprove => Status == ApplicationStatus.Pending;
        public bool CanReject => Status == ApplicationStatus.Pending;
    }
}
