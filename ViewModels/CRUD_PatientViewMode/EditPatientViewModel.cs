using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_PatientViewMode
{
    /// <summary>
    /// View model for editing an existing patient record in the hospital management system.
    /// Allows updating patient demographic and contact information by reception staff.
    /// </summary>
    public class EditPatientViewModel
    {
        public int Id { get; set; }

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

        public bool IsActive { get; set; }
    }
}
