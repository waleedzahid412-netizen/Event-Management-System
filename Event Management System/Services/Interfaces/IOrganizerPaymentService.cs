using Event_Management_System.Models.Entities;

namespace Event_Management_System.Services.Interfaces
{
    public interface IOrganizerPaymentService
    {
        Task<string> CreateOrganizerPaymentAsync(int applicationId);
        Task<OrganizerPayment>ConfirmPaymentAsync(string paymentIntentId);

    }
}
