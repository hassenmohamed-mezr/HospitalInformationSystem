using HospitalInformationSystem.Data;
using HospitalInformationSystem.Helpers;
using HospitalInformationSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalInformationSystem.Controllers
{
    /// <summary>
    /// Controller for user authentication and authorization in the hospital management system.
    /// Manages login, logout, role-based redirection, and access control.
    /// </summary>
    public class AuthController : Controller
    {
        // DB context to access Users table
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Shows login page.
        /// If user already logged in, redirect based on role.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            // Check if session already contains logged-in user
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("RedirectByRole");
            }

            return View();
        }

        /// <summary>
        /// Handles login submission.
        /// Validates credentials and creates session.
        /// </summary>
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Validate form input
            if (!ModelState.IsValid)
                return View(model);

            // Find user by username OR email
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail);

            // SECURITY FIX: Check IsActive to prevent inactive users from authenticating
            // SECURITY FIX: Use generic error message to avoid user enumeration
            if (user == null || !user.IsActive)
            {
                ViewBag.Error = "Invalid credentials.";
                return View(model);
            }

            // Verify hashed password
            bool isPasswordCorrect = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash);

            if (!isPasswordCorrect)
            {
                ViewBag.Error = "Invalid credentials.";
                return View(model);
            }

            // Store user data in session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("RedirectByRole");
        }

        /// <summary>
        /// Redirects user to the correct dashboard based on role.
        /// </summary>
        public IActionResult RedirectByRole()
        {
            var role = HttpContext.Session.GetString("Role");

            // Route user to controller based on role
            return role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Doctor" => RedirectToAction("Index", "Doctor"),
                "Reception" => RedirectToAction("Index", "Reception"),
                _ => RedirectToAction("Login") // fallback
            };
        }

        /// <summary>
        /// Logs out user by clearing session.
        /// </summary>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Displays access denied page.
        /// </summary>
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}