using Event_Management_System.Models.Enums;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;

namespace Event_Management_System.Jobs
{
    public class EventJob
    {
        private readonly IEventRepository _eventRepo;
        private readonly IEmailService _emailService;

        public EventJob(
            IEventRepository eventRepo,
            IEmailService emailService)
        {
            _eventRepo = eventRepo;
            _emailService = emailService;
        }
        public async Task SendCancelEventEmails(int eventId, string cancelReason)
        {
            var eventDetails = await _eventRepo.GetEventDetailsByIdAsync(eventId);

            if (eventDetails == null || eventDetails.Status != EventStatus.Cancelled)
                return;

            var emails = eventDetails.Registrations
                .Where(r => r.User != null && !string.IsNullOrEmpty(r.User.Email))
                .Select(r => r.User.Email)
                .Distinct()
                .ToList();

            foreach (var email in emails)
            {
                await _emailService.SendEventCancellationEmailAsync(
                    email,
                    eventDetails,
                    cancelReason
                );
            }
        }
    }
}
