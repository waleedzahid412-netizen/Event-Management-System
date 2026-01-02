using Event_Management_System.Models.Entities;
using EventManagement.Configuration;

namespace Event_Management_System.Services.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, IEnumerable<string> role);
    }
}
