namespace Event_Management_System.DTOs
{
    public class EventParticipantDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredOn { get; set; }
        public string PaymentStatus { get; set; }
        public bool CheckInStatus { get; set; }
    }
}
