using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.Models
{
    public class Visit
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorProfileId { get; set; }

        public DateTime VisitDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Reason { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public VisitStatus Status { get; set; } = VisitStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public Patient Patient { get; set; } = null!;

        public DoctorProfile DoctorProfile { get; set; } = null!;
    }
}
