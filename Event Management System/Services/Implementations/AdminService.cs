using Event_Management_System.DTOs;
using Event_Management_System.Repositories.Implementations;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;

namespace Event_Management_System.Services.Implementations
{
    public class AdminService : IAdminService
    {
        public readonly IUserRepository _userrepository;
        public readonly IEventRepository _eventRepository;
        public readonly IOrganizerRepository _OrganizerRepository;
        public readonly IRegistrationRepository _registrationRepository;
        public readonly IPaymentRecieptRepository _paymentRecieptRepository;
        public readonly IOrganizerApplicationRepository _organizerApplicationRepository;
        public readonly IAdminAnalyticsRepository _adminAnalyticsRepository;

        public AdminService(IUserRepository userRepository,IEventRepository eventRepository
            ,IOrganizerRepository organizerRepository,IPaymentRecieptRepository paymentRecieptRepository
            ,IOrganizerApplicationRepository organizerApplicationRepository
            ,IRegistrationRepository registrationRepository,IAdminAnalyticsRepository adminAnalyticsRepository)
        {
            _userrepository = userRepository;  
            _eventRepository = eventRepository;
            _OrganizerRepository = organizerRepository;
            _registrationRepository = registrationRepository;
            _paymentRecieptRepository = paymentRecieptRepository;
            _organizerApplicationRepository = organizerApplicationRepository;
            _adminAnalyticsRepository = adminAnalyticsRepository;
        
        }   
        public async Task<AdminDashboardKpiDTO> GetDashboardDataAsync()
        {
            return new AdminDashboardKpiDTO
            {
                TotalUsers = await _userrepository.UserCountAsync(),
                TotalOrganizers = await _OrganizerRepository.OrganizerCountAsync(),
                TotalEvents = await _eventRepository.TotalEventCount(),
                UpcomingEvents = await _eventRepository.UpcomingEventCount(),
                TotalRegistrations = await _registrationRepository.RegistrationCountAsync(),
                TotalRevenue = await _paymentRecieptRepository.TotalRevenueAsync(),
                PendingOrganizerRequests = await _organizerApplicationRepository.TotalOrganizerPendingRequestAsync()


            };
        }
        public async Task<AdminChartsVM> GetAnalytcisDashboardChartsstatsAsync(DateFilterType filter)
        {


            var eventsByStatus = await _adminAnalyticsRepository.GetEventsByStatusAsync(filter);
            var registrationsPerEvent = await _adminAnalyticsRepository.GetRegistrationsPerEventAsync(filter);
            var revenueOverTime = await _adminAnalyticsRepository.GetRevenueOverTimeAsync(filter);
            var upcomingEventsOverTime = await _adminAnalyticsRepository.GetUpcomingEventsOverTimeAsync(filter);
            var avgTicketPriceOverTime = await _adminAnalyticsRepository.GetAvgTicketPriceOverTimeAsync(filter);

            return new AdminChartsVM
            {
                EventsByStatus = eventsByStatus,
                RegistrationsPerEvent = registrationsPerEvent,
                RevenueOverTime = revenueOverTime,
                UpcomingEventsOverTime = upcomingEventsOverTime,
                AvgTicketPriceOverTime = avgTicketPriceOverTime
            };
        }
    }
}
