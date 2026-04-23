using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.Models
{
    public class DoctorProfile
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public required User User { get; set; }

        [Required]
        [StringLength(100)]
        public required string Specialty { get; set; }

        [StringLength(50)]
        public string? Room { get; set; }

        public int? YearsOfExperience { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}
