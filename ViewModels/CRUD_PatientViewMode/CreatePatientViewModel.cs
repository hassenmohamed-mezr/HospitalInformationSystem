using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_PatientViewMode
{
    public class CreatePatientViewModel
    {
        [Required]
        [StringLength(150)]
        public required string FullName { get; set; }

        [DataType(DataType.Date)]
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

        public bool IsActive { get; set; } = true;
    }
}
