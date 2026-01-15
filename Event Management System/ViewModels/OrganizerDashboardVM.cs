using Microsoft.AspNetCore.Routing.Constraints;

namespace Event_Management_System.ViewModels
{
    public enum DateFilterType
    {
        Week,
        Month,
        Year
    }

    public class OrganizerDashboardVM
    {
        // Summary cards
        public int TotalEvents { get; set; }
        public int TotalParticipants { get; set; }
        public double AverageRating { get; set; }
        public decimal TotalEarnings { get; set; }

        // Charts
        public Dictionary<string, int> EventsByStatus { get; set; } = new();
        public Dictionary<string, int> ParticipantsPerEvent { get; set; } = new();
        public Dictionary<string, int> EventsCreatedOverTime { get; set; } = new();
        public Dictionary<string, double> AverageRatingPerEvent { get; set; } = new();

        // Selected filter
        public DateFilterType SelectedFilter { get; set; }
    }
}
