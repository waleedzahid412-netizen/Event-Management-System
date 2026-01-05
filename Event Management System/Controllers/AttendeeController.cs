using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Services;
using Event_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
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


        public async Task<IActionResult> BrowseEvents(int? categoryId, bool showRecommended = false, string status = "upcoming")
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Categories = await _attendeeService.GetAllCategoriesAsync();
            ViewBag.ShowRecommended = showRecommended;
            ViewBag.SelectedTab = status; // Pass the current status to the view

            if (showRecommended)
            {
                // Load recommended events (you might want to implement actual recommendation logic)
                ViewBag.RecommendedEvents = new List<Event_Management_System.Models.Entities.Event>(); // Or load actual recommended events
                return View(new List<Event_Management_System.Models.Entities.Event>());
            }

            var events = await _attendeeService.BrowseEventAsync(categoryId, status);
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
        public async Task<IActionResult> AttendeeEvent(
           int? categoryId,
           bool showRecommended = false,
           string status = "upcoming")
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Categories = await _attendeeService.GetAllCategoriesAsync();
            ViewBag.ShowRecommended = showRecommended;
            ViewBag.status = status;

            var events = await _attendeeService
                .GetEventByAttendeeId(userId, categoryId, showRecommended);

            var today = DateTime.Now;

            events = status == "completed"
                ? events.Where(e => e.EndDate < today).ToList()
                : events.Where(e => e.EndDate >= today).ToList();

            return View(events);
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
        public async Task<IActionResult> OrganizerRegistration()
        {
            int userid=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (await _attendeeService.checkIfApplicationExistButPaymentPending(userid)) {
            var applicationid=await _attendeeService.GetApplicationIdOfUser(userid);
             return RedirectToAction("Pay", "OrganizerPayment", new { applicationid });
            }
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
                var applicationId =await _attendeeService.SubmitOrganizerApplication(dto, userId);
                TempData["Success"] = "Your application has been submitted successfully!";
                return RedirectToAction("Pay", "OrganizerPayment", new { applicationId });
            }
            catch (Exception ex)
            {
                var realError = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", realError);
                return View(dto);


            }

        }
        [HttpGet]
        public async Task<IActionResult> RateEvent(int eventId)
        {
            var ev = await _attendeeService.checkIfEventExists(eventId);
            if (!ev) return NotFound();

            // Optional: check if already rated
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool alreadyRated = await _attendeeService.CheckIfUserReviewExist(eventId,userId);

            if (alreadyRated)
            {
                TempData["Error"] = "You have already rated this event.";
                return RedirectToAction("AttendeeEvent");
            }

            var model = new EventReviewCreateDTO
            {
                EventId = eventId
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RateEvent(EventReviewCreateDTO dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _attendeeService.AddEventReviewAsync(dto, userId);
            }
            catch (Exception ex)
            {
                // Instead of returning raw exception, set TempData and redirect
                TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
                return RedirectToAction("AttendeeEvent");
            }

            TempData["Success"] = "Your review has been submitted successfully!";
            return RedirectToAction("AttendeeEvent");
        }

    }
}
