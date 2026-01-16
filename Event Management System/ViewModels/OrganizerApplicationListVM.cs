using Event_Management_System.Models;

namespace Event_Management_System.ViewModels
{
    public class OrganizerApplicationListVM
    {
        public int Id { get; set; }
        public string ApplicantName { get; set; }
        public string OrganizationName { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedOn { get; set; }

        public bool CanReview => Status == ApplicationStatus.Pending;
    }
}
