using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;


namespace Event_Management_System.Services.Implementations
{
    public class AttendeeService : IAttendeeService
    {
        public readonly IEventRepository _eventRepository;
        public readonly IRegistrationRepository _registrationRepository;
        public readonly IUserRepository _userRepository;
   
        public readonly IPaymentRecieptRepository _paymentrepo;
        public readonly IOrganizerApplicationRepository _organizerApplicationRepository;
        public readonly IEventReviewRepository _eventReviewRepository;
        public readonly IEventCategoryRepository _eventCatregoryRepository;
        public AttendeeService(IEventRepository rep, IRegistrationRepository rrep, 
            IUserRepository userRepository
            , IPaymentRecieptRepository paymentrepo,IOrganizerApplicationRepository repo
            ,IEventReviewRepository eventreviewrepo,IEventCategoryRepository eventcatrepo)
        {

            _registrationRepository = rrep;
            _eventRepository = rep;
            _userRepository = userRepository;

            _paymentrepo = paymentrepo;
            _organizerApplicationRepository = repo;
            _eventReviewRepository= eventreviewrepo;
            _eventCatregoryRepository= eventcatrepo;
        }
        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllEventsWithCategoryAsync();
        }

        public async Task<CustomerEventDetailsDTO> GetEventDetailsAsync(int id)
        {
            var ev = await _eventRepository.GetEventDetailsForCustomerByIdAsync(id);
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


        public async Task<List<EventCategory>> GetAllCategoriesAsync()
        {
            return await _eventCatregoryRepository.GetEventCategoryAsync();
        }

        public async Task<List<Event>> GetEventByAttendeeId(int userid, int? categoryid, bool showrecommended)
        {
            List<Event> events;
            if (showrecommended)
            {
                events=await _eventRepository.GetEventsByAttendeeId(userid);
            }
            else if (categoryid.HasValue)

            {
                events = await _eventRepository.GetEventsByAttendeeIdAndCategoryId(categoryid.Value, userid);



            }
            else
            {
                events = await _eventRepository.GetEventsByAttendeeId(userid);
            }
            return events;

        }
        public async Task<UserProfileDTO> GetUserProfileAsync(int userId, string roleFromToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            return new UserProfileDTO
            {
                UserName = user.FullName,
                Email = user.Email,
                Age = user.age,
                Role = roleFromToken // Use role from JWT claim
            };
        }

        public async Task<int> CountofEventAttendedByUserid(int id)
        {
            return await _eventRepository.CountofEventAttendedByUserid(id);
        }

        public async Task<int> CountofUpcomingEventByUserid(int id)
        {
            return  await _eventRepository.CountofUpcomingEventByUserid(id);
        }





        public async Task<List<Event>> BrowseEventAsync(int? categoryid, string status)
        {
            var events = await _eventRepository.GetAllEventsWithCategoryAsync();
            if (status == "upcoming")
            {
                events = events.Where(e => e.StartDate > DateTime.UtcNow).ToList();
            }
            else { 
            events = events.Where(e => e.EndDate < DateTime.UtcNow).ToList();
            }
            if (categoryid.HasValue)
            {
                events = events.Where(e => e.CategoryId == categoryid.Value).ToList();
            }
            return events;
        }

        public async Task AddEventReviewAsync(EventReviewCreateDTO dto,int userid)
        {
            if (await _eventReviewRepository.CheckIfUserReviewExist(dto.EventId, userid)) { 
                throw new Exception("You have already submitted a review for this event.");
            }
            var review = new EventReview
            {
                EventId = dto.EventId,
                UserId = userid,
                Rating = dto.Rating,
                Comment = dto.Comment
            };
            await _eventReviewRepository.AddEventReview(review);
            await _eventReviewRepository.SaveChangesAsync();

        }

        public async Task<bool> CheckIfUserReviewExist(int Eventid, int userid)
        {
            return await _eventReviewRepository.CheckIfUserReviewExist(Eventid, userid);
        }

        public async Task<bool> checkIfEventExists(int eventid)
        {
            return await _eventRepository.CheckIfEventExists(eventid);
        }





    }
}