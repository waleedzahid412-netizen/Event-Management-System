using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.DTOs
{
    public class LoginViewDTO
    {
        [Required(ErrorMessage = "Please enter your Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        
        public string Password { get; set; }
    }
}
