using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_DoctorProfileViewMode
{
    /// <summary>
    /// View model for creating a new doctor profile in the hospital management system.
    /// Captures doctor-specific details to associate with an existing user account.
    /// </summary>
    public class CreateDoctorProfileViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Specialty { get; set; }

        [StringLength(50)]
        public string? Room { get; set; }

        public int? YearsOfExperience { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
