using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Models.Enums;
using Event_Management_System.Repositories.Implementations;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;

namespace Event_Management_System.Services.Implementations
{
    public class EventService : IEventService
    {
        public readonly ICloudinaryService _cloudinary;
        public readonly IEventRepository _eventrepo;
        public readonly IEventImageRepository _eventimagerepo;
        public readonly IEmailService _emailService;
        public readonly IRegistrationRepository _registrationRepository;
        public EventService(ICloudinaryService cloud,IEventImageRepository eventImageRepository,IEventRepository erepo
            ,IEmailService emailservice,IRegistrationRepository regrepo) 
        { 
        _cloudinary = cloud;
        _eventimagerepo = eventImageRepository;
        _eventrepo = erepo;
        _emailService = emailservice;
        _registrationRepository = regrepo;
        }
        public async Task CreateEventAsync(CreateEventDTO dto, int organizerId)
        {
            var coverurl = await _cloudinary.UploadImageAsync(dto.CoverImage);
            var ev = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Location = dto.Location,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalSeats = dto.TotalSeats,
                TicketPrice = dto.TicketPrice,
                AvailableSeats = dto.TotalSeats,
                OrganizerId = organizerId,
                CoverImageUrl = coverurl,

            };
            await _eventrepo.AddEventAsync(ev);
            await _eventrepo.SaveChangesAsync();

            if (dto.VenueImages != null && dto.VenueImages.Any())
            {
                foreach (var image in dto.VenueImages)
                {
                    var imageurls = await _cloudinary.UploadImageAsync(image);
                    var eventImage = new EventImage
                    {
                        EventId = ev.EventId,
                        ImageUrl = imageurls,
                    };
                    await _eventimagerepo.AddeventImage(eventImage);


                }
                await _eventimagerepo.SaveChangesAsync();

            }
        }
        public async Task<OrganizerEventDetailsDTO> GetOrganizerEventDetailsAsync(int eventId)
        {
            var ev = await _eventrepo.GetEventDetailsByIdAsync(eventId);
            return new OrganizerEventDetailsDTO
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
                VenueImages = ev.EventImages.Select(i => i.ImageUrl).ToList()
            };
        }
        public async Task<bool> UpdateEventStatus(EventStatus status, int eventid)
        {
            var ev = await _eventrepo.GetEventbyIdAsync(eventid);
            if (ev == null)
            {
                return false;
            }
            ev.Status = status;

            await _eventrepo.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SendCancelEventEmailToTheParticipantsAsync(int eventid, string cancelreason)
        {
            var eventdetails = await _eventrepo.GetEventDetailsByIdAsync(eventid);
            if (eventdetails == null)
            {
                return false;
            }
            var uniqueEmails = eventdetails.Registrations
                              .Where(r => r.User != null && !string.IsNullOrEmpty(r.User.Email))
                              .Select(r => r.User.Email)
                              .Distinct()
                              .ToList();
            try
            {
                foreach (var email in uniqueEmails)
                {
                    await _emailService.SendEventCancellationEmailAsync(email, eventdetails, cancelreason);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<Event>> GetEventsByOrganizerAsync(int organizerId, string status)
        {
            if (status == "upcoming")
            {
                return await _eventrepo.GetUpcomingEventByOrganizerIdAsync(organizerId);
            }
            else
            {
                return await _eventrepo.GetCompletedEventByOrganizerIdAsync(organizerId);
            }
        }
        public async Task<CustomerEventDetailsDTO> GetCustomerEventDetailsAsync(int id)
        {
            var ev = await _eventrepo.GetEventDetailsForCustomerByIdAsync(id);
            var eventDetailsDto = new CustomerEventDetailsDTO
            {
                EventId = ev.EventId,
                Title = ev.Title,
                Description = ev.Description,
                Location = ev.Location,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                TotalSeats = ev.TotalSeats,
                TicketPrice = ev.TicketPrice,
                AvailableSeats = ev.AvailableSeats,
                CategoryName = ev.Category.CategoryName,

                CoverImageUrl = ev.CoverImageUrl,
                VenueImages = ev.EventImages.Select(img => img.ImageUrl).ToList()
            };
            return eventDetailsDto;
        }
        public async Task RegisterForEventAsync(int userId, int eventId, int nooftickets)
        {

            var transaction = await _eventrepo.BeginTransactionAsync();
            try
            {
                int countoftickets = await _eventrepo.GetAvailableSeatsAsync(eventId);
                if (countoftickets < nooftickets)
                {
                    throw new Exception("Not enough available seats for the event.");
                }
                for (int i = 0; i < nooftickets; i++)
                {

                    var registration = new Registration
                    {
                        UserId = userId,
                        EventId = eventId,
                        RegisteredOn = DateTime.UtcNow,
                        PaymentStatus = "Pending",

                        TicketNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),

                    };

                    await _registrationRepository.AddRegistrationAsync(registration);

                }
                var ev = await _eventrepo.GetEventDetailsByIdAsync(eventId);
                if (ev != null)
                {
                    ev.AvailableSeats = ev.AvailableSeats - nooftickets;
                }


                await _registrationRepository.SaveChangesAsync();
                await _eventrepo.CommitTransactionAsync(transaction);
            }
            catch
            {
                await _eventrepo.RollBackTransactionAsync(transaction);
                throw;
            }
        }
        public async Task<List<Registration>> GetRegistrationsForUserEventAsync(int userId, int eventId)
        {
            return await _registrationRepository.GetRegistrationsForUserEventAsync(userId, eventId);
        }
        public async Task<List<Event>> BrowseEventAsync(int? categoryid, string status)
        {
            var events = await _eventrepo.GetAllEventsWithCategoryAsync();
            if (status == "upcoming")
            {
                events = events.Where(e => e.StartDate > DateTime.UtcNow).ToList();
            }
            else
            {
                events = events.Where(e => e.EndDate < DateTime.UtcNow).ToList();
            }
            if (categoryid.HasValue)
            {
                events = events.Where(e => e.CategoryId == categoryid.Value).ToList();
            }
            return events;
        }
        public async Task<List<Event>> GetEventByAttendeeId(int userid, int? categoryid, bool showrecommended)
        {
            List<Event> events;
            if (showrecommended)
            {
                events = await _eventrepo.GetEventsByAttendeeId(userid);
            }
            else if (categoryid.HasValue)

            {
                events = await _eventrepo.GetEventsByAttendeeIdAndCategoryId(categoryid.Value, userid);



            }
            else
            {
                events = await _eventrepo.GetEventsByAttendeeId(userid);
            }
            return events;

        }
    }
}
