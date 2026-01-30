using Event_Management_System.DTOs;
using Event_Management_System.Jobs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;
using Hangfire;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

            var ev = await _eventService.GetCustomerEventDetailsAsync(model.EventId);
            if (ev == null)
                return false;


            if (ev.AvailableSeats < model.NumberOfTickets)
                throw new InvalidOperationException("Not enough available seats for the event.");


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
                await _registrationRepo.AddRegistrationAsync(registrations);
                var even =await _eventRepo.GetEventDetailsForCustomerByIdAsync(model.EventId);
                if (even == null) {
                    throw new Exception("Event is mull");
                }

                even.AvailableSeats -= model.NumberOfTickets;


                await _eventRepo.UpdateAsync(even);
                await _registrationRepo.SaveChangesAsync();
                await _eventRepo.CommitTransactionAsync(transaction);


                BackgroundJob.Enqueue<RegistrationEmailJob>(
                    job => job.SendReceiptAndEmailAsync(
                        userId,
                        model.EventId,
                        userEmail
                    )
                );

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
