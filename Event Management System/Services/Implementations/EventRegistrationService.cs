using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;

namespace Event_Management_System.Services.Implementations
{
    public class EventRegistrationService : IEventRegistrationService
    {
        private readonly IEventRepository _eventRepo;
        private readonly IRegistrationRepository _registrationRepo;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IEventService _eventService;
        public EventRegistrationService(
    IEventRepository eventRepo,
    IRegistrationRepository registrationRepo,
    IPaymentService paymentService,
    IEmailService emailService,IEventService eventService  )
        {
            _eventRepo = eventRepo;
            _registrationRepo = registrationRepo;
            _paymentService = paymentService;
            _emailService = emailService;
            _eventService = eventService;
        }
        public async Task<bool> RegisterCustomerAsync(int userId, EventRegistrationDTO model, string userEmail)
        {
            // 1️⃣ Load event
            var ev = await _eventService.GetCustomerEventDetailsAsync(model.EventId);
            if (ev == null)
                return false;

            // 2️⃣ Check available seats
            if (ev.AvailableSeats < model.NumberOfTickets)
                throw new InvalidOperationException("Not enough available seats for the event.");

            // 3️⃣ Begin transaction
            var transaction = await _eventRepo.BeginTransactionAsync();
            try
            {
                // 4️⃣ Create registrations
                var registrations = new List<Registration>();
                for (int i = 0; i < model.NumberOfTickets; i++)
                {
                    registrations.Add(new Registration
                    {
                        UserId = userId,
                        EventId = model.EventId,
                        RegisteredOn = DateTime.UtcNow,
                        PaymentStatus = "Pending",
                        TicketNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                    });
                }

                // 5️⃣ Add registrations to DB
                await _registrationRepo.AddRegistrationAsync(registrations);

                // 6️⃣ Update event available seats
                ev.AvailableSeats -= model.NumberOfTickets;

                // 7️⃣ Save all changes
                await _registrationRepo.SaveChangesAsync();
                await _eventRepo.CommitTransactionAsync(transaction);

                // 8️⃣ Generate PDF receipt
                var registrationDetails = await _registrationRepo.GetRegistrationsForUserEventAsync(userId, model.EventId);
                var result = await _paymentService.CreateCustomerPaymentReceiptAsync(userId, ev, registrationDetails);
                var pdfBytes = result.pdfBytes;


                // 9️⃣ Send confirmation email
                await _emailService.SendConfirmationEmailAsync(userEmail, ev, registrationDetails, pdfBytes, "Receipt.pdf");

                return true;
            }
            catch
            {
                await _eventRepo.RollBackTransactionAsync(transaction);
                throw;
            }
        }

    }
}
