namespace Event_Management_System.DTOs
{
    public class PaymentReceiptDTO
    {
        public string UserEmail { get; set; } = null!;
        public string EventTitle { get; set; } = null!;
        public int NumberOfTickets { get; set; }
        public decimal TicketPrice { get; set; }
        public List<string> TicketNumbers { get; set; } = new();
        public decimal TotalAmount => NumberOfTickets * TicketPrice;
    }
}
