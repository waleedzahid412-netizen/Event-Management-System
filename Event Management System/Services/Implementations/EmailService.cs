using Event_Management_System.Configuration;
using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Services.Interfaces;
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

        public async Task SendConfirmationEmailAsync(string toEmail, CustomerEventDetailsDTO ev, 
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
    }
}
