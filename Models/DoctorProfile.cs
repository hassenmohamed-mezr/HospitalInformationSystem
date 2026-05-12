using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.Models
{
    /// <summary>
    /// Represents a doctor's profile in the hospital management system.
    /// Extends the User model with doctor-specific details such as specialty and room assignment,
    /// and tracks visits managed by the doctor.
    /// </summary>
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

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
