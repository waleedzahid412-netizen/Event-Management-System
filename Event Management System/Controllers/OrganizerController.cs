using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Models.Enums;
using Event_Management_System.Services;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Event_Management_System.Controllers
{
    [Authorize(Roles = "Admin,Organizer")]
    public class OrganizerController : Controller
    {
        public IOrganizerService _organizerService { get; set; }
        public OrganizerController(IOrganizerService organize) {

        _organizerService = organize;    
        }
        public async Task<IActionResult> Dashboard()
        {
            int organizerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            OrganizerDashboardDTO dashboardData = await _organizerService.GetDashboardAsync(organizerId);
            return View(dashboardData);
        }


        public async Task<IActionResult> ViewEventParticipants(int id) { 
        var participants= await _organizerService.GetEventParticipantsAsync(id);
         return View(participants);
        }

        public async Task<IActionResult> AnalyticsDashboard(DateFilterType filter = DateFilterType.Month) {
            int organizerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var analyticsData = await _organizerService.GetAnalyticsDataAsync(organizerId ,filter);
            
            return View(analyticsData);
        }


    
    }
}
