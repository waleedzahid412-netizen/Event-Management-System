using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            var model=await _adminService.GetDashboardDataAsync();
            return View(model);
        }
        public async Task<IActionResult> Analytics(DateFilterType filter = DateFilterType.Week)
        {
            var model = await _adminService.GetAnalytcisDashboardChartsstatsAsync(filter);
            return View(model);
        }


    }
}
