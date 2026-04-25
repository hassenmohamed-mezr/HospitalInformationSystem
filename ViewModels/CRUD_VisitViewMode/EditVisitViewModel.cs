using System.ComponentModel.DataAnnotations;
using HospitalInformationSystem.Models;

namespace HospitalInformationSystem.ViewModels.CRUD_VisitViewMode
{
    /// <summary>
    /// View model for updating an existing patient visit in the hospital management system.
    /// Allows doctors to add notes and change the visit status during or after the appointment.
    /// </summary>
    public class EditVisitViewModel
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public VisitStatus Status { get; set; }
    }
}
