namespace Event_Management_System.DTOs
{
    public class OrganizerPaymentReceiptDTO
    {
        public string UserEmail { get; set; }
        public string OrganizationName { get; set; }
        public int ApplicationId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
