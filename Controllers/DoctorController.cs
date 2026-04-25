using HospitalInformationSystem.Data;
using HospitalInformationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalInformationSystem.Controllers
{

    /// <summary>
    /// Controller for doctor-specific operations in the hospital management system.
    /// Provides dashboard with visit statistics and management access for doctors.
    /// </summary>
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the doctor dashboard with visit statistics.
        /// Shows total, today's, pending, and completed visits, plus the next upcoming visit.
        /// Ensures only users with Doctor role can access.
        /// </summary>
        /// <returns>Doctor index view with visit data or redirects if unauthorized.</returns>
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Doctor")
                return RedirectToAction("AccessDenied", "Auth");

            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("AccessDenied", "Auth");

            var userId = int.Parse(userIdString);

            var doctorProfile = _context.DoctorProfiles
                .FirstOrDefault(d => d.UserId == userId);

            if (doctorProfile == null)
                return RedirectToAction("AccessDenied", "Auth");

            var doctorId = doctorProfile.Id;

            var today = DateTime.Today;

            var visits = _context.Visits
                .Where(v => v.DoctorProfileId == doctorId)
                .ToList();

            ViewBag.TotalVisits = visits.Count;
            ViewBag.TodayVisits = visits.Count(v => v.VisitDate.Date == today);
            ViewBag.PendingVisits = visits.Count(v => v.Status == VisitStatus.Pending);
            ViewBag.CompletedVisits = visits.Count(v => v.Status == VisitStatus.Completed);

            var nextVisit = visits
                .Where(v => v.VisitDate >= DateTime.Now)
                .OrderBy(v => v.VisitDate)
                .FirstOrDefault();

            ViewBag.NextVisit = nextVisit;
            return View();
        }
    }
}
