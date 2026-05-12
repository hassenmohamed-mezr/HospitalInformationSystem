using HospitalInformationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Controllers
{
    /// <summary>
    /// Controller for admin-specific operations in the hospital management system.
    /// Handles administrative dashboard and user management access.
    /// </summary>
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the admin dashboard with comprehensive KPIs and system overview.
        /// Ensures only users with Admin role can access this view.
        /// </summary>
        /// <returns>Admin index view or redirects to access denied if unauthorized.</returns>
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Auth");

            var today = DateTime.Today;

            // KPI Data
            var totalUsers = _context.Users.Count();
            var activeUsers = _context.Users.Count(u => u.IsActive);
            ViewBag.TotalUsers = totalUsers;
            ViewBag.ActiveUsers = activeUsers;
            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.TotalDoctors = _context.DoctorProfiles.Count();
            var todayVisits = _context.Visits.Count(v => v.VisitDate.Date == today);
            ViewBag.TodayVisits = todayVisits;
            ViewBag.TotalAppointments = _context.Appointments.Count();
            ViewBag.PendingAppointments = _context.Appointments.Count(a => a.Status == Models.AppointmentStatus.Pending);
            var newPatientsToday = _context.Patients.Count(p => p.CreatedAt.Date == today);
            var appointmentsScheduledToday = _context.Appointments.Count(a => a.AppointmentDateTime.Date == today);
            ViewBag.SystemActivityToday = todayVisits + newPatientsToday + appointmentsScheduledToday;
            ViewBag.ActiveUserRatePercent = totalUsers == 0 ? 0 : (int)Math.Round(100.0 * activeUsers / totalUsers);

            // Recent Users (last 5)
            var recentUsers = _context.Users.OrderByDescending(u => u.Id).Take(5).ToList();
            ViewBag.RecentUsers = recentUsers;

            // Recent Patients (last 5)
            var recentPatients = _context.Patients.OrderByDescending(p => p.CreatedAt).Take(5).ToList();
            ViewBag.RecentPatients = recentPatients;

            // Recent Visits (today and tomorrow)
            var recentVisits = _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.DoctorProfile)
                .ThenInclude(dp => dp.User)
                .Where(v => v.VisitDate.Date >= today && v.VisitDate.Date <= today.AddDays(1))
                .OrderByDescending(v => v.VisitDate)
                .Take(5)
                .ToList();
            ViewBag.RecentVisits = recentVisits;

            return View();
        }
    }
}