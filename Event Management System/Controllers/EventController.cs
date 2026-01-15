using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Models.Enums;
using Event_Management_System.Services.Implementations;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Event_Management_System.Controllers
{

    public class EventController : Controller
    {
        public readonly IEventCategoryService _eventCategoryService;
        public readonly IEventService _eventService;
        public readonly IPaymentService _paymentService;
        public readonly IEmailService _emailService;
        public EventController(IEventCategoryService eventCategoryservice,  IEventService eventservice,IPaymentService paymentservice, IEmailService emailService) { 
        _eventCategoryService = eventCategoryservice;
        _eventService = eventservice;
        _paymentService = paymentservice;
        _emailService = emailService;

        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> CreateEvent()
        {
            var categories = await _eventCategoryService.GetAllEventCategoryAsync();


            ViewBag.Categories = categories;

            return View(new CreateEventDTO());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _eventCategoryService.GetAllEventCategoryAsync() ?? new List<EventCategory>();
                ViewBag.Categories = categories;

                return View(dto);

            }
            int organizerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _eventService.CreateEventAsync(dto, organizerId);

            return RedirectToAction("MyEvents");

        }
        public async Task<IActionResult> CancelEvent(int eventId)
        {
            int organizerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var eventDto = await _eventService.GetOrganizerEventDetailsAsync(eventId);

            if (eventDto == null) return NotFound();


            var model = new CancelEventVM
            {
                EventId = eventDto.EventId,
                Title = eventDto.Title,
                Description = eventDto.Description,
                Venue = eventDto.Location,
                EventDate = eventDto.StartDate,
                Category = eventDto.CategoryName,
                TotalParticipants = eventDto.TotalSeats - eventDto.AvailableSeats,
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelEvent(CancelEventVM dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Provide Correct Data";
                return View(dto);
            }

            try
            {
                var statusUpdated = await _eventService
                    .UpdateEventStatus(EventStatus.Cancelled, dto.EventId);

                if (!statusUpdated)
                {
                    TempData["Error"] = "Event not found.";
                    return View(dto);
                }

                var emailSent = await _eventService
                    .SendCancelEventEmailToTheParticipantsAsync(dto.EventId, dto.CancelReason);

                if (!emailSent)
                {
                    TempData["Warning"] =
                        "Event cancelled, but some participants could not be notified.";
                }
                else
                {
                    TempData["Success"] =
                        "Event cancelled successfully and participants have been notified.";
                }

                return RedirectToAction("Dashboard");
            }
            catch
            {
                TempData["Error"] =
                    "An error occurred while cancelling the event. Please try again.";
                return View(dto);
            }
        }
        public async Task<IActionResult> OrganizedEvents(string status = "upcoming")
        {
            int organizerid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            IEnumerable<Event> events;
            events = await _eventService.GetEventsByOrganizerAsync(organizerid, status);

            ViewBag.SelectedTab = status;
            return View(events);
        }
        public async Task<IActionResult> EventDetails(int id)
        {
            var eventdetails = await _eventService.GetOrganizerEventDetailsAsync(id);
            return View(eventdetails);
        }
        [HttpGet]
        public async Task<IActionResult> RegisterEvent(int id)
        {
            var ev = await _eventService.GetCustomerEventDetailsAsync(id);

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
            var ev = await _eventService.GetCustomerEventDetailsAsync(dto.EventId);
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

            await _eventService.RegisterForEventAsync(userId, dto.EventId, dto.NumberOfTickets);

            var registration = await _eventService.GetRegistrationsForUserEventAsync(userId, dto.EventId);
            var (receiptEntity, pdfBytes) = await _paymentService.CreateCustomerPaymentReceiptAsync(userId, ev, registration);
            var email = User.FindFirstValue(ClaimTypes.Email);
            await _emailService.SendConfirmationEmailAsync(email, ev, registration,
                pdfBytes, "Receipt.pdf");
            TempData["Success"] = "You have successfully registered for this event!";
            return RedirectToAction("BrowseEvents");
        }
        public async Task<IActionResult> BrowseEvents(int? categoryId, bool showRecommended = false, string status = "upcoming")
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Categories = await _eventCategoryService.GetAllEventCategoryAsync();
            ViewBag.ShowRecommended = showRecommended;
            ViewBag.SelectedTab = status; // Pass the current status to the view

            if (showRecommended)
            {
                // Load recommended events (you might want to implement actual recommendation logic)
                ViewBag.RecommendedEvents = new List<Event_Management_System.Models.Entities.Event>(); // Or load actual recommended events
                return View(new List<Event_Management_System.Models.Entities.Event>());
            }

            var events = await _eventService.BrowseEventAsync(categoryId, status);
            return View(events);
        }
        public async Task<IActionResult> UserEvents(
   int? categoryId,
   bool showRecommended = false,
   string status = "upcoming")
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Categories = await _eventCategoryService.GetAllEventCategoryAsync();
            ViewBag.ShowRecommended = showRecommended;
            ViewBag.status = status;

            var events = await _eventService
                .GetEventByAttendeeId(userId, categoryId, showRecommended);

            var today = DateTime.Now;

            events = status == "completed"
                ? events.Where(e => e.EndDate < today).ToList()
                : events.Where(e => e.EndDate >= today).ToList();

            return View(events);
        }
    }
}
