using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Services;
using Event_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace Event_Management_System.Controllers
{
    public class AttendeeController : Controller
    {
        private readonly IAttendeeService _attendeeService;
        private readonly IEmailService _emailService;


        public AttendeeController(IAttendeeService attendeeService, IEmailService emailService)
        {
            _attendeeService = attendeeService;
            _emailService = emailService;
        }
        public async Task<IActionResult> Dashboard()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var countofupcomingevents = await _attendeeService.CountofUpcomingEventByUserid(id);
            var countofregisteredevents = await _attendeeService.CountofEventAttendedByUserid(id);
            ViewBag.CountofUpcomingEvents = countofupcomingevents;
            ViewBag.CountofRegisteredEvents = countofregisteredevents;
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


        public async Task<IActionResult> EventDetail(int id)
        {
            var eventDetails = await _attendeeService.GetEventDetailsAsync(id);
            if (eventDetails == null)
            {
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
            ViewBag.TicketPrice = ev.TicketPrice;

            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RegisterEvent(EventRegistrationDTO dto)
        {
            var ev = await _attendeeService.GetEventDetailsAsync(dto.EventId);
            if (ev == null)
            {
                ModelState.AddModelError("", "Event not found.");
                return Content("Event not found.");
            }
            ViewBag.EventTitle = ev?.Title;
            ViewBag.CoverImageUrl = ev?.CoverImageUrl;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

                return Content(string.Join(" | ", errors));
            }
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _attendeeService.RegisterForEventAsync(userId, dto.EventId, dto.NumberOfTickets);

            var registration = await _attendeeService.GetRegistrationsForUserEventAsync(userId, dto.EventId);
            var (receiptEntity, pdfBytes) = await _attendeeService.CreatePaymentReceiptAsync(userId, ev, registration);
            var email = User.FindFirstValue(ClaimTypes.Email);
            await _emailService.SendConfirmationEmailAsync(email, ev, registration,
                pdfBytes, "Receipt.pdf");
            TempData["Success"] = "You have successfully registered for this event!";
            return RedirectToAction("BrowseEvents");
        }
        [HttpGet]
        public async Task<IActionResult> AttendeeEvent(int? categoryid, bool showrecommended = false)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Categories = await _attendeeService.GetAllCategoriesAsync();
            ViewBag.ShowRecommended = showrecommended;
            var allEvents = await _attendeeService.GetEventByAttendeeId(userid, categoryid, showrecommended);
            return View(allEvents);

        }

        [HttpGet]
        public async Task<IActionResult> ProfileInfo()
        {
            // Get userId and role from JWT claims
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role); // role from token

            // Pass both to the service
            var profile = await _attendeeService.GetUserProfileAsync(userId, role);

            return View(profile); // pass DTO to view
        }
        [HttpGet]
        public IActionResult OrganizerRegistration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> OrganizerRegistration(OrganizerApplicationCreateDTO dto)

        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _attendeeService.SubmitOrganizerApplication(dto, userId);
                TempData["Success"] = "Your application has been submitted successfully!";
                return RedirectToAction("Dashboard", "Attendee");
            }
            catch (Exception ex)
            {
                var realError = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", realError);
                return View(dto);


            }






        }
    }
}
