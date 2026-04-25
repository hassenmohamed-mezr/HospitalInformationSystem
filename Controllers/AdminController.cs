using Microsoft.AspNetCore.Mvc;

namespace HospitalInformationSystem.Controllers
{
    /// <summary>
    /// Controller for admin-specific operations in the hospital management system.
    /// Handles administrative dashboard and user management access.
    /// </summary>
    public class AdminController : Controller
    {
        /// <summary>
        /// Displays the admin dashboard.
        /// Ensures only users with Admin role can access this view.
        /// </summary>
        /// <returns>Admin index view or redirects to access denied if unauthorized.</returns>
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Auth");

            return View();
        }
    }
}