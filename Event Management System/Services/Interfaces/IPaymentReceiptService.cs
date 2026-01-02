using Event_Management_System.DTOs;
using Event_Management_System.Models;

namespace Event_Management_System.Services.Interfaces
{
    public interface IPaymentReceiptService
    {
        public byte[] GenerateReceiptPdf(PaymentReceiptDTO receipt);
    }
}
