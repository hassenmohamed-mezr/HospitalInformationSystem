using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.Models
{
    /// <summary>
    /// Represents an appointment booking in the hospital management system.
    /// Links a patient to a doctor for a scheduled time slot, with status tracking.
    /// Created by reception/admin, managed by doctors.
    /// </summary>
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorProfileId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        public int? Duration { get; set; } = 30; // Minutes

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public DoctorProfile DoctorProfile { get; set; } = null!;
    }
}