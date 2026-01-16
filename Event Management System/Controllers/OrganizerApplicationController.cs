using Event_Management_System.DTOs;
using Event_Management_System.Models;
using Event_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Event_Management_System.Controllers
{
    public class OrganizerApplicationController : Controller
    {
        public readonly IAttendeeService _attendeeService;
        public readonly IOrganizerApplicationService _organizerApplicationService;
        public OrganizerApplicationController(IAttendeeService attendeeService,IOrganizerApplicationService organizerApplicationService)
        {
            _attendeeService = attendeeService;
            _organizerApplicationService = organizerApplicationService;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> OrganizerRegistration()
        {
            int userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (await _organizerApplicationService.checkIfApplicationExistButPaymentPending(userid))
            {
                var applicationid = await _organizerApplicationService.GetApplicationIdOfUser(userid);
                return RedirectToAction("Pay", "OrganizerPayment", new { applicationid });
            }
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrganizerRegistration(OrganizerApplicationCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var applicationId =
                    await _organizerApplicationService.SubmitOrganizerApplication(dto, userId);

                TempData["Success"] =
                    "Your application has been submitted successfully!";

                return RedirectToAction(
                    "Pay",
                    "OrganizerPayment",
                    new { applicationId });
            }
            catch (Exception ex)
            {
                var realError = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError(string.Empty, realError);
                return View(dto);
            }
        }
        public async Task<IActionResult> ViewOrganizerApplications(ApplicationStatus status =ApplicationStatus.Pending) {

            var applications =
                await _organizerApplicationService.GetApplicationsByStatus(status);

            ViewBag.CurrentStatus = status;
            return View(applications);
        }
        public async Task<IActionResult> OrganizerApplicationDetails(int id)
        {
            var vm = await _organizerApplicationService.GetApplicationDetailsAsync(id);
            if (vm == null)
                return NotFound();

            return View(vm);
        }
     
        [HttpPost]
        public async Task<IActionResult> ApproveApplication(int applicationId, int userId, string ApplicantEmail)

        {
            int adminid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _organizerApplicationService.ApproveApplicationAsync(applicationId, userId,ApplicantEmail,adminid);

            if (!success) return NotFound();

            return RedirectToAction("ViewOrganizerApplications");
        }

        [HttpPost]
        public async Task<IActionResult> RejectApplication(int applicationId, string ApplicantEmail ,string comments)
        {
            int adminid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _organizerApplicationService.RejectApplicationAsync(applicationId, ApplicantEmail, adminid, comments);

            if (!success) return NotFound();

            return RedirectToAction("ViewOrganizerApplications");
        }



    }
}
