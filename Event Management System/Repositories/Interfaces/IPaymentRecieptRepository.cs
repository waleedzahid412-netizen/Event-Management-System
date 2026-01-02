using Event_Management_System.Models.Entities;

namespace Event_Management_System.Repositories.Interfaces
{
    public interface IPaymentRecieptRepository
    {
        public Task AddPaymentReciept(PaymentReciept payment);
        public Task SaveChangesAsync();
    }
}
