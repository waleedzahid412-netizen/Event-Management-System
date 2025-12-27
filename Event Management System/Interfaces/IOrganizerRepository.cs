namespace Event_Management_System.Interfaces
{
    public interface IOrganizerRepository
    {
        Task<int> GetTotalEventsAsync(int organizerId);
        Task<int> GetUpcomingEventsAsync(int organizerId);
        Task<int> GetActiveEventsAsync(int organizerId);
        Task<int> GetTotalRegistrationsAsync(int organizerId);
    }
}
