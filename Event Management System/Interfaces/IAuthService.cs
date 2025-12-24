using Event_Management_System.DTOs;

namespace Event_Management_System.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginViewDTO dto);
        Task RegisterAsync(RegisterViewDTO dto); 
    }
}
