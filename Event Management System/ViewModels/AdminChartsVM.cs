namespace Event_Management_System.ViewModels
{
    public class AdminChartsVM
    {
        public DateFilterType SelectedFilter { get; set; }

        // Charts Data
        public Dictionary<string, int> EventsByStatus { get; set; } = new();
        public Dictionary<string, int> RegistrationsPerEvent { get; set; } = new();
        public Dictionary<string, decimal> RevenueOverTime { get; set; } = new();
        public Dictionary<string, int> UpcomingEventsOverTime { get; set; } = new();
        public Dictionary<string, decimal> AvgTicketPriceOverTime { get; set; } = new();
    }
}
