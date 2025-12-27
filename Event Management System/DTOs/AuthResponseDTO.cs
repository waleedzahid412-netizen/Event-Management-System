namespace Event_Management_System.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> roles = new List<string>();
    }
}
