namespace Event_Management_System.DTOs
{
    public class UserProfileDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Role { get; set; } // Added user role
    }
}
