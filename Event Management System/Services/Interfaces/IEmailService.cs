using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.ViewModels;

namespace Event_Management_System.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendConfirmationEmailAsync(string toEmail,
           EventDetailsVM ev,
            List<Registration> registrations, byte[]? pdfAttachment = null, string attachmentName = "Receipt.pdf");

        public Task SendOrganizerApplicationEmailAsync(
    string toEmail,
    OrganizerApplication application,
    byte[] receiptPdf);
        public Task SendEventCancellationEmailAsync(string email, Event eventdetails,string cancelreason);
        public  Task OrganizerApprovalEmail(int applicationId, string toEmail);
        public Task OrganizerRejectionEmail(OrganizerApplication application, string toEmail);
    }

   
}
