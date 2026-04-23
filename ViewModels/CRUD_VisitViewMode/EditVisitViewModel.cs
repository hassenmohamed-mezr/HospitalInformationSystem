using System.ComponentModel.DataAnnotations;
using HospitalInformationSystem.Models;

namespace HospitalInformationSystem.ViewModels.CRUD_VisitViewMode
{
    public class EditVisitViewModel
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public VisitStatus Status { get; set; }
    }
}
