using Event_Management_System.Configuration;
using Event_Management_System.DTOs;
using Event_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Event_Management_System.Controllers
{
    [Authorize(Roles ="Organizer")]
    public class OrganizerPaymentController : Controller
    {
        private readonly IOrganizerPaymentService _paymentService;
        private readonly IOptions<StripeSettings> _stripeOptions;
        private readonly IAttendeeService _attendeeService;
        public readonly IPaymentService _receiptService;
        public readonly IEmailService _emailService;
        public readonly IOrganizerApplicationService _organizerApplicationService;
        public OrganizerPaymentController(IOrganizerPaymentService paymentService, IOptions<StripeSettings> stripeOptions
            ,IAttendeeService service, IPaymentService receiptService, IEmailService emailService, IOrganizerApplicationService organizerApplicationService)
        {
            _paymentService = paymentService;
            _stripeOptions = stripeOptions;
            _attendeeService = service;
            _receiptService = receiptService;
            _emailService = emailService;
            _organizerApplicationService = organizerApplicationService;
        }

        public async Task<IActionResult> Pay(int applicationId)

        {
            var dto=await _organizerApplicationService.GetOrganizerApplicationByIdAsync(applicationId);
            var clientSecret = await _paymentService.CreateOrganizerPaymentAsync(applicationId);
            ViewBag.PublishableKey = _stripeOptions.Value.PublishableKey;
            ViewBag.ClientSecret = clientSecret;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(string paymentIntentId)
        {
            var payment=await _paymentService.ConfirmPaymentAsync(paymentIntentId);
            var emailofuser = User.FindFirstValue(ClaimTypes.Email);
            var receiptDto = new OrganizerPaymentReceiptDTO
            {
                UserEmail = emailofuser,
                OrganizationName = payment.OrganizerApplication.OrganizationName,
                ApplicationId = payment.OrganizerApplicationId,
                AmountPaid = payment.Amount,
                PaymentDate = DateTime.UtcNow
            };

            // Generate receipt PDF
            var receiptPdf = _receiptService.GenerateOrganizerReceiptPdf(receiptDto);

            // Send email
            await _emailService.SendOrganizerApplicationEmailAsync(
                receiptDto.UserEmail,
                payment.OrganizerApplication,
                receiptPdf
            );

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

    }
}
