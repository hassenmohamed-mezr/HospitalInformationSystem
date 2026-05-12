using HospitalInformationSystem.Data;
using HospitalInformationSystem.Models;
using HospitalInformationSystem.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Controllers
{

    /// <summary>
    /// Controller for doctor-specific operations in the hospital management system.
    /// Provides comprehensive dashboard with appointment and visit statistics.
    /// </summary>
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the doctor dashboard with comprehensive KPIs and daily schedule.
        /// Shows appointments, visits, and relevant statistics for the logged-in doctor.
        /// Ensures only users with Doctor role can access.
        /// </summary>
        /// <returns>Doctor index view with detailed metrics or redirects if unauthorized.</returns>
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Doctor")
                return RedirectToAction("AccessDenied", "Auth");

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("AccessDenied", "Auth");

            var userId = int.Parse(userIdString);
            var doctorProfile = _context.DoctorProfiles.FirstOrDefault(d => d.UserId == userId);

            if (doctorProfile == null)
                return RedirectToAction("AccessDenied", "Auth");

            var doctorId = doctorProfile.Id;
            var today = DateTime.Today;

            // KPI: Appointments Data
            var allAppointments = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorProfileId == doctorId)
                .ToList();

            var todayAppointments = allAppointments
                .Where(a => a.AppointmentDateTime.Date == today && a.Status != AppointmentStatus.Cancelled)
                .ToList();

            ViewBag.TotalAppointments = allAppointments.Count();
            ViewBag.TodayAppointments = todayAppointments.Count;
            ViewBag.PendingAppointments = allAppointments.Count(a => a.Status == AppointmentStatus.Pending);
            ViewBag.ConfirmedAppointments = allAppointments.Count(a => a.Status == AppointmentStatus.Confirmed);

            // KPI: Visits Data
            var allVisits = _context.Visits
                .Where(v => v.DoctorProfileId == doctorId)
                .ToList();

            ViewBag.TotalVisits = allVisits.Count;
            ViewBag.TodayVisits = allVisits.Count(v => v.VisitDate.Date == today);
            ViewBag.PendingVisits = allVisits.Count(v => v.Status == VisitStatus.Pending);
            ViewBag.CompletedVisits = allVisits.Count(v => v.Status == VisitStatus.Completed);

            // Today's Schedule (appointments + visits)
            var todaySchedule = todayAppointments
                .OrderBy(a => a.AppointmentDateTime)
                .ToList();

            ViewBag.TodaySchedule = todaySchedule;

            var todayVisitsForTimeline = _context.Visits
                .Include(v => v.Patient)
                .Where(v => v.DoctorProfileId == doctorId && v.VisitDate.Date == today)
                .ToList();

            var timeline = new List<DoctorScheduleLineViewModel>();
            foreach (var a in todayAppointments.OrderBy(x => x.AppointmentDateTime))
            {
                timeline.Add(new DoctorScheduleLineViewModel
                {
                    At = a.AppointmentDateTime,
                    Kind = "Appointment",
                    PatientName = a.Patient?.FullName ?? "Patient",
                    EntityId = a.Id,
                    DetailController = "Appointments",
                    DetailAction = "Details",
                    AppointmentStatus = a.Status
                });
            }

            foreach (var v in todayVisitsForTimeline.OrderBy(x => x.VisitDate))
            {
                timeline.Add(new DoctorScheduleLineViewModel
                {
                    At = v.VisitDate,
                    Kind = "Visit",
                    PatientName = v.Patient?.FullName ?? "Patient",
                    EntityId = v.Id,
                    DetailController = "Visits",
                    DetailAction = "Edit",
                    VisitStatus = v.Status
                });
            }

            ViewBag.TodayTimeline = timeline.OrderBy(t => t.At).ToList();

            // Next Appointment
            var nextAppointment = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorProfileId == doctorId && a.AppointmentDateTime >= DateTime.Now && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.AppointmentDateTime)
                .FirstOrDefault();

            ViewBag.NextAppointment = nextAppointment;

            // Next Visit (if any)
            var nextVisit = _context.Visits
                .Include(v => v.Patient)
                .Where(v => v.DoctorProfileId == doctorId && v.VisitDate >= DateTime.Now)
                .OrderBy(v => v.VisitDate)
                .FirstOrDefault();

            ViewBag.NextVisit = nextVisit;

            var recentTreatedVisits = _context.Visits
                .Include(v => v.Patient)
                .Where(v => v.DoctorProfileId == doctorId && v.Status == VisitStatus.Completed)
                .OrderByDescending(v => v.VisitDate)
                .Take(5)
                .ToList();

            ViewBag.RecentTreatedVisits = recentTreatedVisits;

            return View();
        }
    }
}
