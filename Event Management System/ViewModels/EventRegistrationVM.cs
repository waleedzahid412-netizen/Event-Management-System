namespace Event_Management_System.ViewModels
{
    public class EventRegistrationVM
    {
    
        public int EventId { get; set; }
        public string Title { get; set; }
        public string CoverImageUrl { get; set; }
        public decimal TicketPrice { get; set; }
        public int NumberOfTickets { get; set; } = 1;
        public string PaymentStatus { get; set; } = "Pending";
        public List<string>? TicketNumbers { get; set; }
    }
}
