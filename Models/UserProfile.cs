using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.Models
{
    public class UserProfile
    {
        public int Id { get; set; }


        [Required]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(300)]
        public string? Bio { get; set; }

        [StringLength(255)]
        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}