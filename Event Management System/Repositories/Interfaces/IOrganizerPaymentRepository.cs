using Event_Management_System.Models.Entities;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IOrganizerPaymentRepository
    {
        Task AddAsync(OrganizerPayment payment);
        Task<OrganizerPayment?> GetByPaymentIntentIdAsync(string paymentIntentId);
        Task UpdateAsync(OrganizerPayment payment);
        public Task<bool> CheckIfPaymentEntryAlreadyExist(int applicationId);
    }
}
