using EventManagement.Configuration;
using EventManagement.Models;

namespace Event_Management_System.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, IEnumerable<string> role);
    }
}
