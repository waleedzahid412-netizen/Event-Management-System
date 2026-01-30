using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;

namespace Event_Management_System.Jobs
{
    public class RegistrationEmailJob
    {
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IRegistrationRepository _registrationRepo;
        private readonly IEventRepository _eventRepo;
        private readonly IEventService _eventService;

        public RegistrationEmailJob(
            IPaymentService paymentService,
            IEmailService emailService,
            IRegistrationRepository registrationRepo,
            IEventRepository eventRepo,
            IEventService eventService)
        {
            _paymentService = paymentService;
            _emailService = emailService;
            _registrationRepo = registrationRepo;
            _eventRepo = eventRepo;
            _eventService = eventService;
        }

        public async Task SendReceiptAndEmailAsync(
            int userId,
            int eventId,
            string userEmail)
        {
            var ev = await _eventService.GetCustomerEventDetailsAsync(eventId);
            var registrations = await _registrationRepo
                .GetRegistrationsForUserEventAsync(userId, eventId);

            var (receipt, pdfBytes) =
                await _paymentService.CreateCustomerPaymentReceiptAsync(
                    userId, ev, registrations);

            await _emailService.SendConfirmationEmailAsync(
                userEmail,
                ev,
                registrations,
                pdfBytes,
                "Receipt.pdf"
            );
        }
    }
}

