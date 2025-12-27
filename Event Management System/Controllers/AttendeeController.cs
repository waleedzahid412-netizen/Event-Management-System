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
    public class AttendeeController : Controller
    {
        private readonly IAttendeeService _attendeeService;

        public AttendeeController(IAttendeeService attendeeService)
        {
            _attendeeService = attendeeService;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> BrowseEvents(int? categoryId, bool showRecommended = false)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            ViewBag.Categories = await _attendeeService.GetAllCategoriesAsync();
            ViewBag.ShowRecommended = showRecommended;

            if (showRecommended)
            {

                return View(new List<Event>());
            }
            var events = await _attendeeService.GetAllEventsAsync();


            if (categoryId.HasValue)
            {
                events = events.Where(e => e.CategoryId == categoryId.Value).ToList();
            }
           
            return View(events);
        }


        public async Task<IActionResult> EventDetail(int id) { 
            var eventDetails = await _attendeeService.GetEventDetailsAsync(id);
            if (eventDetails == null) {
                return NotFound();
            }
            return View(eventDetails);

        }
        [HttpGet]
        public async Task<IActionResult> RegisterEvent(int id)
        {
            var ev = await _attendeeService.GetEventDetailsAsync(id);

            if (ev == null)
                return NotFound();

            // Prepare DTO for the form
            var dto = new EventRegistrationDTO
            {
                EventId = id,
                PaymentStatus = "Pending" // default
            };

            ViewBag.EventTitle = ev.Title;
            ViewBag.CoverImageUrl = ev.CoverImageUrl;

            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RegisterEvent(EventRegistrationDTO dto)
        {
            var ev = await _attendeeService.GetEventDetailsAsync(dto.EventId);
            ViewBag.EventTitle = ev?.Title;
            ViewBag.CoverImageUrl = ev?.CoverImageUrl;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

                return Content(string.Join(" | ", errors));
            }

            int userId;
            try
            {
                userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch
            {
                var errors = ModelState.Values
               .SelectMany(v => v.Errors)
               .Select(e => e.ErrorMessage);

                return Content(string.Join(" | ", errors));

            }

            try
            {
                await _attendeeService.RegisterForEventAsync(userId, dto.EventId);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }

            TempData["Success"] = "You have successfully registered for this event!";
            return RedirectToAction("BrowseEvents");
        }








    }
}
