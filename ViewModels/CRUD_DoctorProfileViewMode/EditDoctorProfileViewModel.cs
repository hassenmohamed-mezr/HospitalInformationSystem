using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_DoctorProfileViewMode
{
    public class EditDoctorProfileViewModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Specialty { get; set; }

        [StringLength(50)]
        public string? Room { get; set; }

        public int? YearsOfExperience { get; set; }

        public bool IsActive { get; set; }
    }
}
