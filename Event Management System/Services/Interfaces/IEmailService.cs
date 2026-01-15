using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;

namespace Event_Management_System.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendConfirmationEmailAsync(string toEmail,
            CustomerEventDetailsDTO ev,
            List<Registration> registrations, byte[]? pdfAttachment = null, string attachmentName = "Receipt.pdf");

        public Task SendOrganizerApplicationEmailAsync(
    string toEmail,
    OrganizerApplication application,
    byte[] receiptPdf);
        public Task SendEventCancellationEmailAsync(string email, Event eventdetails,string cancelreason);
    }

   
}
