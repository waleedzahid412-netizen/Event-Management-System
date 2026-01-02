using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Event_Management_System.Models.Entities
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required, MaxLength(50)]
        public string RoleName { get; set; }

        // Navigation
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
