using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.DTOs
{
    public class RegisterViewDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain at least one uppercase and one special character"
        )]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Range(13, 120, ErrorMessage = "Age must be greater than 12")]
        public int age { get; set; }
    }
}
