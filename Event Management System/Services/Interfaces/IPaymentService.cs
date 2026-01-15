using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;

namespace Event_Management_System.Services.Interfaces
{
    public interface IPaymentService
    {
        public  Task<(PaymentReciept receipt, byte[] pdfBytes)> CreateCustomerPaymentReceiptAsync(
             int userId,
             CustomerEventDetailsDTO ev,
             List<Registration> registrations);
        public byte[] GenerateCustomerRegistationReceiptPdf(PaymentReceiptDTO receipt);
        public byte[] GenerateOrganizerReceiptPdf(OrganizerPaymentReceiptDTO receipt);
    }
}
