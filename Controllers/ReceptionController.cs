using HospitalInformationSystem.Data;
using HospitalInformationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Controllers
{
    /// <summary>
    /// Controller for reception-specific operations in the hospital management system.
    /// Provides comprehensive dashboard with patient, visit, and appointment statistics.
    /// </summary>
    public class ReceptionController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ReceptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the reception dashboard with comprehensive KPIs and daily operations overview.
        /// Shows patient, appointment, and visit statistics with actionable insights.
        /// Ensures only users with Reception role can access.
        /// </summary>
        /// <returns>Reception index view with detailed metrics or redirects if unauthorized.</returns>
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Reception")
                return RedirectToAction("AccessDenied", "Auth");

            var today = DateTime.Today;

            // KPI: Patients
            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.NewPatients = _context.Patients.Count(p => p.CreatedAt.Date == today);
            ViewBag.ActivePatients = _context.Patients.Count(p => p.IsActive);

            // KPI: Appointments
            var todayAppointments = _context.Appointments
                .Where(a => a.AppointmentDateTime.Date == today)
                .ToList();

            ViewBag.TotalAppointments = _context.Appointments.Count();
            ViewBag.TodayAppointments = todayAppointments.Count;
            ViewBag.PendingAppointments = todayAppointments.Count(a => a.Status == AppointmentStatus.Pending);
            ViewBag.ConfirmedAppointments = todayAppointments.Count(a => a.Status == AppointmentStatus.Confirmed);
            ViewBag.CompletedAppointments = todayAppointments.Count(a => a.Status == AppointmentStatus.Completed);

            // KPI: Visits
            var todayVisits = _context.Visits
                .Where(v => v.VisitDate.Date == today)
                .ToList();

            ViewBag.TotalVisits = _context.Visits.Count();
            ViewBag.TodayVisits = todayVisits.Count;
            ViewBag.PendingVisits = todayVisits.Count(v => v.Status == VisitStatus.Pending);
            ViewBag.CompletedVisits = todayVisits.Count(v => v.Status == VisitStatus.Completed);

            // Today's Appointment Queue (sorted by time)
            var appointmentQueue = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.DoctorProfile)
                .ThenInclude(dp => dp.User)
                .Where(a => a.AppointmentDateTime.Date == today && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.AppointmentDateTime)
                .Take(10)
                .ToList();

            ViewBag.AppointmentQueue = appointmentQueue;

            // Recently Registered Patients (last 5)
            var recentPatients = _context.Patients
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToList();

            ViewBag.RecentPatients = recentPatients;

            return View();
        }
    }
}