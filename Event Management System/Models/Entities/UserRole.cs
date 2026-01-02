namespace Event_Management_System.Models.Entities
{
    public class UserRole
    {
        public int UserRoleId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Navigation
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
