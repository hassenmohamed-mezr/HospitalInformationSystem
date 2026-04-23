using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_VisitViewMode
{
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
