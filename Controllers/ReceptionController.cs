using HospitalInformationSystem.Models;
using HospitalInformationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Controllers
{
    /// <summary>
    /// Controller for reception-specific operations in the hospital management system.
    /// Provides dashboard with patient and visit statistics for reception staff.
    /// </summary>
    public class ReceptionController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ReceptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the reception dashboard with patient and visit statistics.
        /// Shows total patients, today's visits, pending visits, and completed visits.
        /// Ensures only users with Reception role can access.
        /// </summary>
        /// <returns>Reception index view with statistics.</returns>
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Reception")
                return RedirectToAction("AccessDenied", "Auth");

            var today = DateTime.Today;

            var visits = _context.Visits.ToList();

            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.TodayVisits = visits.Count(v => v.VisitDate.Date == today);
            ViewBag.PendingVisits = visits.Count(v => v.Status == VisitStatus.Pending);
            ViewBag.CompletedVisits = visits.Count(v => v.Status == VisitStatus.Completed);

            return View();
        }
    }
}