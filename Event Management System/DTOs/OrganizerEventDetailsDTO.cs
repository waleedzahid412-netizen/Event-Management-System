namespace Event_Management_System.DTOs
{
    public class OrganizerEventDetailsDTO
    {
        // Event Info
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string CategoryName { get; set; }
        public string CoverImageUrl { get; set; }

        public List<string> VenueImages { get; set; }

        // Participants
        public List<EventParticipantDTO> Participants { get; set; }
    }
}
