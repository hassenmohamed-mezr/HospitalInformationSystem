using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public required string Role { get; set; } // Admin, Doctor, Reception
        public bool IsActive { get; set; } = true;

        public UserProfile? UserProfile { get; set; }


    }
}