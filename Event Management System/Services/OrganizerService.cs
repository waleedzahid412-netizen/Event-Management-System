using Event_Management_System.DTOs;
using Event_Management_System.Interfaces;
using EventManagement.Models;

namespace Event_Management_System.Services
{
    public class OrganizerService : IOrganizerService
    {
        public readonly IOrganizerRepository _organizerRepo;
        public readonly ICloudinaryService _cloudinary;
        public readonly IEventRepository _eventrepo;
        public readonly IEventImageRepository _eventimagerepo;
        public OrganizerService(IOrganizerRepository organizer,ICloudinaryService cloud
            , IEventRepository evrep, IEventImageRepository eventimage)
        {
            
                _organizerRepo = organizer;
                _cloudinary = cloud;
                _eventrepo = evrep;
                _eventimagerepo = eventimage;

            

            
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
                AvailableSeats = dto.TotalSeats,
                OrganizerId=organizerId,
                CoverImageUrl=coverurl,

            };
            await _eventrepo.AddEventAsync(ev);
            await _eventrepo.SaveChangesAsync();

            if (dto.VenueImages != null && dto.VenueImages.Any()) {
                foreach (var image in dto.VenueImages) {
                    var imageurls= await _cloudinary.UploadImageAsync(image);
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

        public Task<List<EventCategory>> GetAllEventCategoryAsync()
        {
            return _eventrepo.GetEventCategoryAsync();
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

        public async Task<OrganizerEventDetailsDTO> GetEventDetailsAsync(int eventId)
        {
            var ev= await _eventrepo.GetEventDetailsByIdAsync(eventId);
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
        public async Task<List<EventParticipantDTO>> GetEventParticipantsAsync(int eventId)
        {
            var registrations = await _eventrepo.GetEventParticipantsbyEventIdAsync(eventId);

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
            public async Task<List<Event>> GetEventsByOrganizerAsync(int organizerId,string status)
            {
                if (status == "upcoming")
                {
                    return await _eventrepo.GetUpcomingEventByOrganizerIdAsync(organizerId);
                }
                else {
                    return await _eventrepo.GetCompletedEventByOrganizerIdAsync(organizerId);
                        }
                }
    } }
