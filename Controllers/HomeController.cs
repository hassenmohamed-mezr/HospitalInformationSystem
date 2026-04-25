using System.Diagnostics;
using HospitalInformationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalInformationSystem.Controllers
{
    /// <summary>
    /// Controller for general pages in the hospital management system.
    /// Handles the home page, privacy policy, and error displays.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the home page of the hospital management system.
        /// </summary>
        /// <returns>Home index view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// </summary>
        /// <returns>Privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays error details for debugging purposes.
        /// Prevents caching to ensure fresh error information.
        /// </summary>
        /// <returns>Error view with request details.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
