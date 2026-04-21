using HospitalInformationSystem.Data;
using HospitalInformationSystem.Models;
using HospitalInformationSystem.ViewModels.CRUD_DoctorProfileViewMode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Controllers
{
    public class DoctorsController : Controller
    {
        // DB context for doctor profiles and users
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if current user is Admin.
        /// </summary>
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        /// <summary>
        /// Detects if DB exception is caused by UNIQUE constraint.
        /// </summary>
        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException?.Message.Contains("UNIQUE") == true
                || ex.Message.Contains("UNIQUE");
        }

        /// <summary>
        /// Displays all doctor profiles with related user info.
        /// </summary>
        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            // Include User data (JOIN)
            var Doctors = _context.DoctorProfiles
                .Include(dp => dp.User)
                .OrderBy(dp => dp.User.FullName)
                .ToList();

            return View(Doctors);
        }

        /// <summary>
        /// Search Doctor by FullName and Specialty.
        /// </summary>

        public async Task<IActionResult> Search(string? fullName, string? specialty)
        {

            var query = _context.DoctorProfiles
                .Include(d => d.User)
                .AsNoTracking()
                .AsQueryable();

            // Filter by doctor name (from User)
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var term = fullName.Trim();
                query = query.Where(d => d.User.FullName.Contains(term));
            }

            // Filter by specialty (direct field)
            if (!string.IsNullOrWhiteSpace(specialty))
            {
                var term = specialty.Trim();
                query = query.Where(d => d.Specialty.Contains(term));
            }

            ViewBag.FullName = fullName;
            ViewBag.Specialty = specialty;

            var doctors = await query
                .OrderBy(d => d.User.FullName)
                .ToListAsync();

            return View("Index",doctors);
        }

        /// <summary>
        /// Shows create doctor profile form.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            // Get users already assigned to profiles
            var existingDoctorUserIds = _context.DoctorProfiles.Select(dp => dp.UserId).ToList();

            // Filter users: only active doctors without profile
            var doctorUsers = _context.Users
                .Where(u => u.Role == "Doctor" && u.IsActive && !existingDoctorUserIds.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FullName} ({u.Username})"
                })
                .ToList();

            // Pass dropdown data to view
            ViewBag.DoctorUsers = doctorUsers;

            return View();
        }

        /// <summary>
        /// Handles create doctor profile.
        /// </summary>
        [HttpPost]
        public IActionResult Create(CreateDoctorProfileViewModel model)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            // Rebuild dropdown list (needed if validation fails)
            var existingDoctorUserIds = _context.DoctorProfiles.Select(dp => dp.UserId).ToList();
            ViewBag.DoctorUsers = _context.Users
                .Where(u => u.Role == "Doctor" && u.IsActive && !existingDoctorUserIds.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FullName} ({u.Username})"
                })
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            // Validate selected user
            var user = _context.Users.Find(model.UserId);
            if (user == null || user.Role != "Doctor")
            {
                ModelState.AddModelError(nameof(model.UserId), "Please select a valid doctor user.");
                return View(model);
            }

            // Ensure one profile per doctor
            bool profileExists = _context.DoctorProfiles.Any(dp => dp.UserId == model.UserId);
            if (profileExists)
            {
                ModelState.AddModelError(nameof(model.UserId), "This doctor already has a profile.");
                return View(model);
            }

            // Create profile entity
            var profile = new DoctorProfile
            {
                UserId = model.UserId,
                User = user,
                Specialty = model.Specialty.Trim(),
                Room = string.IsNullOrWhiteSpace(model.Room) ? null : model.Room.Trim(),
                YearsOfExperience = model.YearsOfExperience,
                IsActive = model.IsActive
            };

            _context.DoctorProfiles.Add(profile);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Handle DB unique constraint safely
                if (IsUniqueConstraintViolation(ex))
                {
                    ModelState.AddModelError(nameof(model.UserId), "This doctor already has a profile.");
                    return View(model);
                }

                throw;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Loads edit form for doctor profile.
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            // Load profile with user
            var profile = _context.DoctorProfiles
                .Include(dp => dp.User)
                .FirstOrDefault(dp => dp.Id == id);

            if (profile == null) return NotFound();

            // Display doctor name in view
            ViewBag.DoctorName = $"{profile.User.FullName} ({profile.User.Username})";

            var model = new EditDoctorProfileViewModel
            {
                Id = profile.Id,
                UserId = profile.UserId,
                Specialty = profile.Specialty,
                Room = profile.Room,
                YearsOfExperience = profile.YearsOfExperience,
                IsActive = profile.IsActive
            };

            return View(model);
        }

        /// <summary>
        /// Handles edit doctor profile.
        /// </summary>
        [HttpPost]
        public IActionResult Edit(EditDoctorProfileViewModel model)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var profile = _context.DoctorProfiles
                .Include(dp => dp.User)
                .FirstOrDefault(dp => dp.Id == model.Id);

            if (profile == null) return NotFound();

            // Keep doctor name for UI
            ViewBag.DoctorName = $"{profile.User.FullName} ({profile.User.Username})";

            // Update fields
            profile.Specialty = model.Specialty.Trim();
            profile.Room = string.IsNullOrWhiteSpace(model.Room) ? null : model.Room.Trim();
            profile.YearsOfExperience = model.YearsOfExperience;
            profile.IsActive = model.IsActive;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (IsUniqueConstraintViolation(ex))
                {
                    ModelState.AddModelError(nameof(model.UserId), "This doctor already has a profile.");
                    return View(model);
                }

                throw;
            }

            return RedirectToAction("Index");
        }
    }
}