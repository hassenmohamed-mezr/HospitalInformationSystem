using HospitalInformationSystem.Models;

namespace HospitalInformationSystem.ViewModels.Dashboard
{
    public class DoctorScheduleLineViewModel
    {
        public DateTime At { get; set; }
        public string Kind { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string DetailController { get; set; } = string.Empty;
        public string DetailAction { get; set; } = "Details";
        public AppointmentStatus? AppointmentStatus { get; set; }
        public VisitStatus? VisitStatus { get; set; }
    }
}
