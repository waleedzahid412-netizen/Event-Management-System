using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using EventManagement.Data;

namespace Event_Management_System.Repositories.Implementations
{
    public class PaymentRecieptRepository : IPaymentRecieptRepository
    {
        ApplicationDbContext _context;
        public PaymentRecieptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPaymentReciept(PaymentReciept payment)
        {
            await _context.PaymentReceipts.AddAsync(payment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
