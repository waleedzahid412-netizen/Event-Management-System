using Event_Management_System.ViewModels;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IAdminAnalyticsRepository
    {
        Task<Dictionary<string, int>> GetEventsByStatusAsync(DateFilterType filter);
        Task<Dictionary<string, int>> GetRegistrationsPerEventAsync(DateFilterType filter);
        Task<Dictionary<string, decimal>> GetRevenueOverTimeAsync(DateFilterType filter);
        Task<Dictionary<string, int>> GetUpcomingEventsOverTimeAsync(DateFilterType filter);
        Task<Dictionary<string, decimal>> GetAvgTicketPriceOverTimeAsync(DateFilterType filter);
    }
}
