using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Models.Enums;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;

namespace Event_Management_System.Services.Implementations
{
    public class OrganizerService : IOrganizerService
    {
        public readonly IOrganizerRepository _organizerRepo;
        public readonly ICloudinaryService _cloudinary;
        public readonly IEventRepository _eventrepo;
        public readonly IEventImageRepository _eventimagerepo;
        public readonly IRegistrationRepository _registrationrepo;
        public readonly IEmailService _emailService;
        public OrganizerService(IOrganizerRepository organizer,ICloudinaryService cloud
            , IEventRepository evrep,IEventImageRepository eventimage,
            IRegistrationRepository regrep,IEmailService ser
            )
        {
            
            _organizerRepo = organizer;
            _cloudinary = cloud;
            _eventrepo = evrep;
            _eventimagerepo = eventimage;
            _registrationrepo = regrep;
            _emailService = ser;



        }

        public async Task<OrganizerDashboardVM> GetAnalyticsDataAsync(int userid,  DateFilterType filter)

        {   DateTime startdate= filter switch
            {
                DateFilterType.Week => DateTime.Now.AddDays(-7),
                DateFilterType.Month => DateTime.Now.AddMonths(-1),
                DateFilterType.Year => DateTime.Now.AddYears(-1),
                _ => DateTime.Now.AddDays(-7)
            };
            var events= await  _eventrepo.GetEventInSpecificTimeFrameByOrganizerIdAsync(userid, startdate);
            return new OrganizerDashboardVM
            {
                TotalEvents = events.Count,
                TotalParticipants = events.Sum(e => e.Registrations.Count),
                AverageRating = events.Where(e => e.Reviews.Any()).Any() ? events.Where(e => e.Reviews.Any()).Average(e => e.Reviews.Average(r => r.Rating)) : 0,
                TotalEarnings = events.Sum(e =>
                {
                    var soldSeats = e.TotalSeats - e.AvailableSeats;
                    return soldSeats > 0 ? soldSeats * e.TicketPrice : 0;
                }),
                EventsByStatus = events.GroupBy(e => e.EndDate < DateTime.Now ? "Completed" : "Upcoming/Active  ")
                                      .ToDictionary(g => g.Key, g => g.Count()),
                ParticipantsPerEvent = events.ToDictionary(e => e.Title, e => e.Registrations.Count),
                EventsCreatedOverTime = events.GroupBy(e => e.StartDate.ToString("yyyy-MM"))
                                              .ToDictionary(g => g.Key, g => g.Count()),
                AverageRatingPerEvent = events.Where(e => e.Reviews.Any())
                                              .ToDictionary(e => e.Title, e => e.Reviews.Average(r => r.Rating)),
                SelectedFilter = filter

            };

        }

        public async Task<OrganizerDashboardDTO> GetDashboardAsync(int organizerId)
        {
            return new OrganizerDashboardDTO
            {
                TotalEvents = await _organizerRepo.GetTotalEventsAsync(organizerId),
                UpcomingEvents = await _organizerRepo.GetUpcomingEventsAsync(organizerId),
                ActiveEvents = await _organizerRepo.GetActiveEventsAsync(organizerId),
                TotalRegistrations = await _organizerRepo.GetTotalRegistrationsAsync(organizerId)
            };

    }


        public async Task<List<EventParticipantDTO>> GetEventParticipantsAsync(int eventId)
        {
            var registrations = await _registrationrepo.GetEventParticipantsbyEventIdAsync(eventId);
            Console.WriteLine($"Found {registrations.Count} registrations for EventId {eventId}");

            if (registrations == null || !registrations.Any())
                return new List<EventParticipantDTO>();


            return registrations
                .Select(r => new EventParticipantDTO
                {
                    UserName = r.User.FullName,
                    Email = r.User.Email,
                    RegisteredOn = r.RegisteredOn,
                    PaymentStatus = r.PaymentStatus,
                    CheckInStatus = r.CheckInStatus
                })
                .ToList();
        }





    } }
