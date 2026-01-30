using Event_Management_System.DTOs;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Event_Management_System.Controllers
{
    public class EventRegistrationController : Controller
    {
        public readonly IEventService _eventService;
        public readonly IEventRegistrationService _eventRegistrationService;
        public EventRegistrationController(IEventService eventService,IEventRegistrationService regservice) { 
        _eventService= eventService; 
        _eventRegistrationService= regservice;
        }
        [Authorize(Roles = "Customer,Organizer")]
        [HttpGet]
        public async Task<IActionResult> RegisterEvent(int id)
        {
            var model = await _eventService.GetEventRegistrationAsync(id);

            if (model == null)
                return NotFound();

            return View(model);

        }
        [Authorize(Roles = "Customer,Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RegisterEvent(EventRegistrationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return Content(string.Join(" | ", errors));
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            try
            {
                bool success = await _eventRegistrationService.RegisterCustomerAsync(userId,dto, userEmail);

                if (success)
                {
                    TempData["Success"] = "You have successfully registered for this event!";
                    return RedirectToAction("BrowseEvents","Event");
                }
                else
                {
                    return Content("Registration failed. Event may no longer exist.");
                }
            }
            catch (InvalidOperationException ex)
            {
                return Content(ex.Message); 
            }
            catch (Exception)
            {
                return Content("An unexpected error occurred during registration.");
            }
        }

    }
}
