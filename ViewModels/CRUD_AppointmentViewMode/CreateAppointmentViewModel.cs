using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_AppointmentViewMode
{
    public class CreateAppointmentViewModel
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorProfileId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDateTime { get; set; }

        [Range(15, 120)]
        public int Duration { get; set; } = 30;

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}