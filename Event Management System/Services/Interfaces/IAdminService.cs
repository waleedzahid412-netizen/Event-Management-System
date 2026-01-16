using Event_Management_System.DTOs;
using Event_Management_System.ViewModels;

namespace Event_Management_System.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardKpiDTO> GetDashboardDataAsync();
        public Task<AdminChartsVM> GetAnalytcisDashboardChartsstatsAsync(DateFilterType filter);
    }
}
