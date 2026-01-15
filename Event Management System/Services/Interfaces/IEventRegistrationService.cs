using Event_Management_System.DTOs;
using Event_Management_System.ViewModels;

namespace Event_Management_System.Services.Interfaces
{
    public interface IEventRegistrationService
    {
        public  Task<bool> RegisterCustomerAsync(int userId, EventRegistrationDTO model, string userEmail);
    }
}
