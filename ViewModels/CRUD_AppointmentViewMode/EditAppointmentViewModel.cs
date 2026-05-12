using HospitalInformationSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_AppointmentViewMode
{
    public class EditAppointmentViewModel
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorProfileId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDateTime { get; set; }

        [Range(15, 120)]
        public int Duration { get; set; } = 30;

        [Required]
        public AppointmentStatus Status { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}