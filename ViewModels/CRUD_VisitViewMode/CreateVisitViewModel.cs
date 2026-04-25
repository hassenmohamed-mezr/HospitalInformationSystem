using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_VisitViewMode
{
    /// <summary>
    /// View model for scheduling a new patient visit in the hospital management system.
    /// Associates a patient with a doctor, sets the appointment date and reason, handled by reception.
    /// </summary>
    public class CreateVisitViewModel
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorProfileId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime VisitDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Reason { get; set; }
    }
}
