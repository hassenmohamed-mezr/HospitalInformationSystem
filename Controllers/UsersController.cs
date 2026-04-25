using HospitalInformationSystem.Data;
using HospitalInformationSystem.Helpers;
using HospitalInformationSystem.Models;
using HospitalInformationSystem.ViewModels.CRUD_UserViewModel;
using Microsoft.AspNetCore.Mvc;

namespace HospitalInformationSystem.Controllers
{
    /// <summary>
    /// Controller for managing users in the hospital management system.
    /// Provides CRUD operations for user accounts, accessible only to admin.
    /// Ensures unique usernames and emails, with password hashing and soft deactivation.
    /// </summary>
    public class UsersController : Controller
    {
        // Database context to perform CRUD operations on Users
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if the current logged-in user has Admin role.
        /// Used to restrict access to controller actions.
        /// </summary>
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        /// <summary>
        /// Displays a list of all users.
        /// Accessible only by Admin.
        /// </summary>
        public IActionResult Index()
        {
            // Block non-admin users
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            // Retrieve all users from database
            var users = _context.Users.ToList();

            return View(users);
        }

        /// <summary>
        /// Returns the Create User form.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            return View();
        }

        /// <summary>
        /// Handles Create User form submission.
        /// Validates input, checks duplicates, and saves user.
        /// </summary>
        [HttpPost]
        public IActionResult Create(CreateUserViewModel model)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            // Validate form inputs (DataAnnotations)
            if (!ModelState.IsValid)
                return View(model);

            // Check if username already exists in DB
            bool usernameExists = _context.Users
                .Any(u => u.Username == model.Username);

            // Check if email already exists in DB
            bool emailExists = _context.Users
                .Any(u => u.Email == model.Email);

            // Prevent duplicate username
            if (usernameExists)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(model);
            }

            // Prevent duplicate email
            if (emailExists)
            {
                ModelState.AddModelError("", "Email already exists.");
                return View(model);
            }

            // Map ViewModel to User entity
            var user = new User
            {
                FullName = model.FullName,
                Username = model.Username,
                Email = model.Email,

                // Always store hashed password (security best practice)
                PasswordHash = PasswordHelper.HashPassword(model.Password),

                Role = model.Role,
                IsActive = model.IsActive
            };

            // Add and persist new user
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Loads user data into Edit form.
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            // Find user by primary key
            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            // Map User entity to ViewModel
            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return View(model);
        }

        /// <summary>
        /// Handles Edit User form submission.
        /// Updates user data after validation.
        /// </summary>
        [HttpPost]
        public IActionResult Edit(EditUserViewModel model)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            // Validate form data
            if (!ModelState.IsValid)
                return View(model);

            // Retrieve existing user from DB
            var user = _context.Users.Find(model.Id);

            if (user == null)
                return NotFound();

            // Ensure username is unique (excluding current user)
            bool usernameExists = _context.Users
                .Any(u => u.Username == model.Username && u.Id != model.Id);

            // Ensure email is unique (excluding current user)
            bool emailExists = _context.Users
                .Any(u => u.Email == model.Email && u.Id != model.Id);

            if (usernameExists)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(model);
            }

            if (emailExists)
            {
                ModelState.AddModelError("", "Email already exists.");
                return View(model);
            }

            // Apply updated values
            user.FullName = model.FullName;
            user.Username = model.Username;
            user.Email = model.Email;
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            // Save changes to database
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Deactivates a user (soft delete).
        /// Prevents disabling the main admin account.
        /// </summary>
        public IActionResult Deactivate(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            // Protect default admin from being disabled
            if (user.Username == "admin")
            {
                TempData["Error"] = "Default admin account cannot be deactivated.";
                return RedirectToAction("Index");
            }

            // Mark user as inactive instead of deleting
            user.IsActive = false;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}