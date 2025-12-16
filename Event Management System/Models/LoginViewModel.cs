using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your username")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        
        public string Password { get; set; }
    }
}
