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
        public readonly IPaymentReceiptService _paymentReceiptService;
        public readonly IPaymentRecieptRepository _paymentrepo;
        public readonly IOrganizerApplicationRepository _organizerApplicationRepository;
        public AttendeeService(IEventRepository rep, IRegistrationRepository rrep, 
            IUserRepository userRepository, IPaymentReceiptService paymentReceiptService
            , IPaymentRecieptRepository paymentrepo,IOrganizerApplicationRepository repo)
        {

            _registrationRepository = rrep;
            _eventRepository = rep;
            _userRepository = userRepository;
            _paymentReceiptService = paymentReceiptService;
            _paymentrepo = paymentrepo;
            _organizerApplicationRepository = repo;
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
        public async Task RegisterForEventAsync(int userId, int eventId, int nooftickets)
        {

            var transaction = await _eventRepository.BeginTransactionAsync();
            try
            {
                int countoftickets = await _eventRepository.GetAvailableSeatsAsync(eventId);
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
                var ev = await _eventRepository.GetEventDetailsByIdAsync(eventId);
                if (ev != null)
                {
                    ev.AvailableSeats = ev.AvailableSeats - nooftickets;
                }


                await _registrationRepository.SaveChangesAsync();
                await _eventRepository.CommitTransactionAsync(transaction);
            }
            catch
            {
                await _eventRepository.RollBackTransactionAsync(transaction);
                throw;
            }
        }

        public async Task<List<EventCategory>> GetAllCategoriesAsync()
        {
            return await _eventRepository.GetEventCategoryAsync();
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

        public async Task<List<Registration>> GetRegistrationsForUserEventAsync(int userId, int eventId)
        {
            return await _registrationRepository.GetRegistrationsForUserEventAsync(userId, eventId);
        }
        public async Task<(PaymentReciept receipt, byte[] pdfBytes)> CreatePaymentReceiptAsync(
                           int userId,
                           CustomerEventDetailsDTO ev,
                           List<Registration> registrations
    )
        {

            var receiptData = new PaymentReceiptDTO
            {
                UserEmail = await _registrationRepository.useremail(userId),
                EventTitle = ev.Title,
                NumberOfTickets = registrations.Count,
                TicketPrice = ev.TicketPrice,
                TicketNumbers = registrations.Select(r => r.TicketNumber).ToList()
            };

            var pdfBytes = _paymentReceiptService.GenerateReceiptPdf(receiptData);


            var receiptEntity = new PaymentReciept
            {
                UserId = userId,
                EventId = ev.EventId,
                NumberOfTickets = registrations.Count,
                TicketPrice = ev.TicketPrice,
                TotalAmount = registrations.Count * ev.TicketPrice,
                ReceiptPdf = pdfBytes
            };

            await _paymentrepo.AddPaymentReciept(receiptEntity);
            await _paymentrepo.SaveChangesAsync();

            return(receiptEntity,pdfBytes) ;
        }

        public async Task SubmitOrganizerApplication(OrganizerApplicationCreateDTO dto,int Userid)
        {
            bool haspending= await _organizerApplicationRepository.CheckIfOrganizerApplicationExistAsync(Userid);
            if (haspending) {
               throw new Exception("You have already submitted an application. Please wait for review." );

            }
            var application = new OrganizerApplication
            {
                UserId = Userid,
                OrganizationName = dto.OrganizationName,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                ExperienceDescription = dto.ExperienceDescription,
                WebsiteUrl = dto.WebsiteUrl,
            };
            await _organizerApplicationRepository.AddApplicationAsync(application);
            await _organizerApplicationRepository.SaveChangesAsync();
            
        }
    }
}