using Event_Management_System.DTOs;
using Event_Management_System.Interfaces;
using Event_Management_System.Services;
using EventManagement.Models;
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

        public async Task<IActionResult> CreateEvent() {
            var categories = await _organizerService.GetAllEventCategoryAsync();

          
            ViewBag.Categories = categories;

            return View(new CreateEventDTO());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventDTO dto) {
            if (!ModelState.IsValid) {
                var categories = await _organizerService.GetAllEventCategoryAsync() ?? new List<EventCategory>();
                ViewBag.Categories = categories;

                return View(dto);

            }
            int organizerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _organizerService.CreateEventAsync(dto, organizerId);

            return RedirectToAction("MyEvents");

        }
        public async Task<IActionResult> MyEvents(string status="upcoming") {
            int organizerid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            IEnumerable<Event> events;
            events=await _organizerService.GetEventsByOrganizerAsync(organizerid,status);

            ViewBag.SelectedTab = status;
            return View(events);
        }
        public async Task<IActionResult> EventDetails(int id) {
            var eventdetails= await _organizerService.GetEventDetailsAsync(id);
            return View(eventdetails);
        }
        public async Task<IActionResult> ViewEventParticipants(int eventId) { 
        var participants= await _organizerService.GetEventParticipantsAsync(eventId);
            return View(participants);
        }
    }
}
