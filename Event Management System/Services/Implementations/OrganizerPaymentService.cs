using Event_Management_System.Configuration;
using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;

namespace Event_Management_System.Services.Implementations
{
    public class OrganizerPaymentService : IOrganizerPaymentService

    {
        private readonly IOrganizerPaymentRepository _ogpaymentRepo;
        private readonly StripeSettings _stripeSettings;

        public OrganizerPaymentService(IOrganizerPaymentRepository paymentRepo, IOptions<StripeSettings> options)
        {
            _ogpaymentRepo = paymentRepo;
            _stripeSettings = options.Value;

            // Initialize Stripe with Secret Key
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }



        public async Task<OrganizerPayment> ConfirmPaymentAsync(string paymentIntentId)
        {
            var payment = await _ogpaymentRepo.GetByPaymentIntentIdAsync(paymentIntentId);

            if (payment != null)
            {
                payment.Status = PaymentStatus.Succeeded;
                payment.OrganizerApplication.IsPaymentCompleted
                    = true;

                await _ogpaymentRepo.UpdateAsync(payment);
                return payment;
            }
            return null;

        }

        public async  Task<string> CreateOrganizerPaymentAsync(int applicationId)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = 5000, // $50 in cents
                Currency = _stripeSettings.Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            var payment = new OrganizerPayment
            {
                OrganizerApplicationId = applicationId,
                Amount = 50,
                StripePaymentIntentId = intent.Id,
                Status = PaymentStatus.Pending
            };
            if (!await _ogpaymentRepo.CheckIfPaymentEntryAlreadyExist(applicationId))
            {


                await _ogpaymentRepo.AddAsync(payment);
            }

            // Return client secret to frontend
            return intent.ClientSecret;
        }
    }
}
