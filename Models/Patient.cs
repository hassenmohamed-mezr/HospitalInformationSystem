using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public required string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        [StringLength(50)]
        public string? NationalId { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [Required]
        [StringLength(30)]
        public required string PhoneNumber { get; set; }

        // === Profile Enhancements ===

        [StringLength(255)]
        public string? ProfileImageUrl { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}