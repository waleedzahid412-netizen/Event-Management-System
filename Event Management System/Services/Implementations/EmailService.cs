using Event_Management_System.Configuration;
using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Event_Management_System.Services.Implementations
{
    public class EmailService :IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendConfirmationEmailAsync(string toEmail, EventDetailsVM ev, 
            List<Registration> registrations, byte[]? pdfAttachment = null, string attachmentName = "Receipt.pdf")
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.SmtpUser));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = $"Registration Confirmation for {ev.Title}";

            // Generate ticket numbers list
            var ticketList = string.Join("\n", registrations.ConvertAll(r => r.TicketNumber));
            var totalAmount = registrations.Count * ev.TicketPrice;

            var body = new TextPart("plain")
            {
                Text = $@"
Hello,

You have successfully registered for {ev.Title}.

Number of Tickets: {registrations.Count}
Ticket Numbers:
{ticketList}

Total Amount: ${totalAmount}

Thank you for registering!
"
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);

            // Add PDF attachment if provided
            if (pdfAttachment != null)
            {
                var attachment = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(new MemoryStream(pdfAttachment)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = attachmentName
                };
                multipart.Add(attachment);
            }

            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SmtpUser,_emailSettings.SmtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendEventCancellationEmailAsync(string toEmail, Event eventdetails,string cancelreason)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.SmtpUser));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = $"Event Cancellation Notice: {eventdetails.Title}";

            var body = new TextPart("plain")
            {
                Text = $@"
Dear Participant,

We regret to inform you that the following event has been cancelled:

Event Name: {eventdetails.Title}
Category: {eventdetails.Category?.CategoryName}
Start Date: {eventdetails.StartDate:MMMM dd, yyyy}
Cancellation Reason: {cancelreason}

We sincerely apologize for any inconvenience this may cause.
If you have already registered, further details regarding refunds or next steps will be shared separately.

Best regards,
Event Management Team
"
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);

            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _emailSettings.SmtpServer,
                _emailSettings.SmtpPort,
                MailKit.Security.SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                _emailSettings.SmtpUser,
                _emailSettings.SmtpPass
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendOrganizerApplicationEmailAsync(
    string toEmail,
    OrganizerApplication application,
    byte[] receiptPdf)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.SmtpUser));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Organizer Application Submitted Successfully";

            var body = new TextPart("plain")
            {
                Text = $@"
Hello,

Your organizer application has been submitted successfully.

Organization Name: {application.OrganizationName}
Application ID: {application.OrganizerApplicationId}

Your payment has been received. Please find the receipt attached.

Our admin team will review your application shortly.

Thank you,
Event Management System
"
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);

            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(new MemoryStream(receiptPdf)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "OrganizerPaymentReceipt.pdf"
            };

            multipart.Add(attachment);
            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        public async Task OrganizerApprovalEmail(int applicationId, string toEmail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.SmtpUser));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Organizer Request Approval Confirmation";

            var body = new TextPart("plain")
            {
                Text = $@"
Hello,

We’re excited to inform you that your request to become an Organizer has been approved by the admin!

As an approved organizer, you now have access to a suite of powerful tools to manage and grow your events:

- Create and Manage Events: Easily set up new events, manage registrations, and track attendee details.
- Analytics Dashboard: Gain insights into ticket sales, attendee engagement, and overall event performance.
- Customizable Event Pages: Showcase your events with personalized branding and details.
- Exclusive Organizer Perks: Receive early access to platform features and support to maximize your event success.

We’re thrilled to have you as part of our organizer community and can’t wait to see the amazing events you’ll create!

Thank you for joining us and taking your first step towards hosting unforgettable experiences.

Application ID: {applicationId}
"
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);
            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        public async Task OrganizerRejectionEmail(OrganizerApplication application, string toEmail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.SmtpUser));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Organizer Application Status Update";

            var body = new TextPart("plain")
            {
                Text = $@"
Hello {application.User.FullName},

We wanted to update you regarding your recent request to become an Organizer on our platform.

After careful review, your application has been **rejected** by the admin.

**Admin Comments:**
{application.AdminComments}

We understand this may be disappointing, but we encourage you to review the feedback and consider applying again in the future. We value your interest in being part of our organizer community and hope to see your applications again.

Thank you for your time and understanding.

Application ID: {application.OrganizerApplicationId}
Reviewed On: {application.ReviewedOn:yyyy-MM-dd}
"
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);

            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }




    }
}
