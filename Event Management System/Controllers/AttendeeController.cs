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
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> Dashboard()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var countofupcomingevents = await _attendeeService.CountofUpcomingEventByUserid(id);
            var countofregisteredevents = await _attendeeService.CountofEventAttendedByUserid(id);
            ViewBag.CountofUpcomingEvents = countofupcomingevents;
            ViewBag.CountofRegisteredEvents = countofregisteredevents;
            return View();
        }








        [Authorize]
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


 

    }
}
