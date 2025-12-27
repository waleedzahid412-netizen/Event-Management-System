using Event_Management_System.DTOs;
using Event_Management_System.Interfaces;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Services
{
    public class AttendeeService : IAttendeeService
    {
        IEventRepository _eventRepository;
        IRegistrationRepository _registrationRepository;
        public AttendeeService(IEventRepository rep,IRegistrationRepository rrep) {

            _registrationRepository = rrep;
        _eventRepository= rep;
        
        }    
        public async Task<List<EventManagement.Models.Event>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllEventsWithCategoryAsync();
        }

        public async Task<CustomerEventDetailsDTO> GetEventDetailsAsync(int id)
        {
            var ev= await _eventRepository.GetEventDetailsForCustomerByIdAsync(id);
            var eventDetailsDto = new CustomerEventDetailsDTO
            {
                EventId = ev.EventId,
                Title = ev.Title,
                Description = ev.Description,
                Location = ev.Location,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                TotalSeats = ev.TotalSeats,
                AvailableSeats = ev.AvailableSeats,
                CategoryName = ev.Category.CategoryName,
                CoverImageUrl = ev.CoverImageUrl,
                VenueImages = ev.EventImages.Select(img => img.ImageUrl).ToList()
            };
            return eventDetailsDto;
        }
        public async Task RegisterForEventAsync(int userId, int eventId)
        {
            // Check if user already registered
            bool alreadyRegistered = await  _registrationRepository.IsUserAlreadyRegisteredAsync(userId, eventId);

            if (alreadyRegistered)
                throw new Exception("You are already registered for this event.");

            var registration = new Registration
            {
                UserId = userId,
                EventId = eventId,
                RegisteredOn = DateTime.UtcNow,
                PaymentStatus = "Pending",
                
                TicketNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
            };

            await _registrationRepository.AddRegistrationAsync(registration);
            await _registrationRepository.SaveChangesAsync();
        }

        public async Task<List<EventCategory>> GetAllCategoriesAsync()
        {
            return await _eventRepository.GetEventCategoryAsync();
        }

       
    }
}
