using Event_Management_System.DTOs;
using Event_Management_System.Jobs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Models.Enums;
using Event_Management_System.Services.Implementations;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;
using Hangfire;
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
        public readonly IAttendeeService _attendeeService;
        public readonly IRecommendationService _recommendationService;
        public EventController(IEventCategoryService eventCategoryservice,  IEventService eventservice,
            IPaymentService paymentservice, IEmailService emailService,
            IAttendeeService attendeeService,IRecommendationService recommendationService) { 
        _eventCategoryService = eventCategoryservice;
        _eventService = eventservice;
        _paymentService = paymentservice;
        _emailService = emailService;
        _attendeeService= attendeeService;
        _recommendationService = recommendationService;

        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="Organizer")]
        public async Task<IActionResult> CreateEvent()
        {
            var categories = await _eventCategoryService.GetAllEventCategoryAsync();


            ViewBag.Categories = categories;

            return View(new CreateEventDTO());
        }

        [Authorize(Roles = "Organizer")]
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

            return RedirectToAction("UserEvents");

        }

        [Authorize(Roles = "Organizer")]
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

        [Authorize(Roles = "Organizer")]
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

                BackgroundJob.Enqueue<EventJob>(
                    job => job.SendCancelEventEmails(dto.EventId, dto.CancelReason)
                );


                TempData["Success"] =
                    "Event cancelled successfully. Emails will be sent shortly.";

                return RedirectToAction("Dashboard");
            }
            catch
            {
                TempData["Error"] =
                    "An error occurred while cancelling the event. Please try again.";
                return View(dto);
            }
        }

        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> OrganizedEvents(string status = "upcoming")
        {
            int organizerid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            IEnumerable<Event> events;
            events = await _eventService.GetEventsByOrganizerAsync(organizerid, status);

            ViewBag.SelectedTab = status;
            return View(events);
        }
        [Authorize]
        public async Task<IActionResult> EventDetails(int id)
        {
            // 1️⃣ Get main event details
            var eventDetails =
                await _eventService.GetOrganizerEventDetailsAsync(id);

            if (eventDetails == null)
                return NotFound();

            // 2️⃣ Call Recommendation Microservice
            var recResponse =
                await _recommendationService.GetRecommendationsAsync(
                    eventDetails.EventId,
                    topN: 4);

            // Safety check (ML API may return nothing)
            var recommendedIds = recResponse?.Recommendations?
                .Select(r => r.EventId)
                .ToList() ?? new List<int>();

            // 3️⃣ Fetch full event info from DB (via repo → service)
            var similarEvents = recommendedIds.Any()
                ? await _eventService
                    .GetRecommendedEventDetailsByIdsAsync(recommendedIds)
                : new List<OrganizerEventDetailsDTO>();

            // 4️⃣ Page ViewModel
            var vm = new EventDetailsPageViewModel
            {
                Event = eventDetails,
                SimilarEvents = similarEvents
            };

            return View(vm);
        }
        [Authorize]

        public async Task<IActionResult> BrowseEvents(int? categoryId, bool showRecommended = false)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Categories = await _eventCategoryService.GetAllEventCategoryAsync();
            ViewBag.ShowRecommended = showRecommended;
  

            if (showRecommended)
            {
                // Load recommended events (you might want to implement actual recommendation logic)
                ViewBag.RecommendedEvents = new List<Event_Management_System.Models.Entities.Event>(); // Or load actual recommended events
                return View(new List<Event_Management_System.Models.Entities.Event>());
            }

            var events = await _eventService.BrowseEventAsync(categoryId);
            return View(events);
        }
        [Authorize]
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

            events = status == "Completed"
                ? events.Where(e => e.Status==EventStatus.Completed).ToList()
                : events.Where(e => e.Status == EventStatus.Upcoming).ToList();

            return View(events);
        }
        [Authorize(Roles ="Customer")]
        [HttpGet]
        public async Task<IActionResult> RateEvent(int eventId)
        {
            var ev = await _attendeeService.checkIfEventExists(eventId);
            if (!ev) return NotFound();

            // Optional: check if already rated
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool alreadyRated = await _attendeeService.CheckIfUserReviewExist(eventId, userId);

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
        [Authorize(Roles = "Customer")]
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
            return RedirectToAction("UserEvents");
        }
    }
}
