namespace Event_Management_System.DTOs
{
    public class AdminDashboardKpiDTO
    {
        public int TotalUsers { get; set; }
        public int TotalOrganizers { get; set; }
        public int TotalEvents { get; set; }
        public int UpcomingEvents { get; set; }
        public int TotalRegistrations { get; set; }
        public decimal? TotalRevenue { get; set; }
        public int PendingOrganizerRequests { get; set; }

    }
}
