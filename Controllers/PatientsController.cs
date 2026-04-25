using HospitalInformationSystem.Data;
using HospitalInformationSystem.Models;
using HospitalInformationSystem.ViewModels.CRUD_PatientViewMode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Controllers
{
    /// <summary>
    /// Controller for managing patients in the hospital management system.
    /// Provides CRUD operations for patient records, with role-based access:
    /// Admin and Reception can create/edit, all roles can view.
    /// Ensures unique National ID and valid date of birth.
    /// </summary>
    public class PatientsController : Controller
    {
        // DB context for Patients table
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Allows only Admin and Reception to manage patients.
        /// </summary>
        private bool CanManagePatients()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin" || role == "Reception";
        }

        /// <summary>
        /// Allows Admin, Reception, and Doctor to view patients (read-only for Doctor).
        /// </summary>
        private bool CanViewPatients()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin" || role == "Reception" || role == "Doctor";
        }

        /// <summary>
        /// Cleans National ID (trim or null).
        /// </summary>
        private static string? NormalizeNationalId(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.Trim();
        }

        /// <summary>
        /// Detects UNIQUE constraint errors.
        /// </summary>
        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException?.Message.Contains("UNIQUE") == true
                || ex.Message.Contains("UNIQUE");
        }

        /// <summary>
        /// Displays patients list with optional filters.
        /// </summary>
        public async Task<IActionResult> Index(string? fullName, string? nationalId, string? phoneNumber)
        {
            if (!CanViewPatients())
                return RedirectToAction("AccessDenied", "Auth");

            // Base query without tracking (read-only)
            var query = _context.Patients.AsNoTracking().AsQueryable();

            // Filter by name
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var term = fullName.Trim();
                query = query.Where(p => p.FullName.Contains(term));
            }

            // Filter by National ID
            if (!string.IsNullOrWhiteSpace(nationalId))
            {
                var term = nationalId.Trim();
                query = query.Where(p => p.NationalId != null && p.NationalId.Contains(term));
            }

            // Filter by phone number
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var term = phoneNumber.Trim();
                query = query.Where(p => p.PhoneNumber.Contains(term));
            }

            // Keep filter values in view
            ViewBag.FullName = fullName;
            ViewBag.NationalId = nationalId;
            ViewBag.PhoneNumber = phoneNumber;

            var patients = await query.OrderBy(p => p.FullName).ToListAsync();
            return View(patients);
        }

        /// <summary>
        /// Shows create patient form.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            if (!CanManagePatients())
                return RedirectToAction("AccessDenied", "Auth");

            return View();
        }

        /// <summary>
        /// Handles patient creation.
        /// </summary>
        [HttpPost]
        public IActionResult Create(CreatePatientViewModel model)
        {
            if (!CanManagePatients())
                return RedirectToAction("AccessDenied", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            // Prevent future date of birth
            if (model.DateOfBirth.HasValue && model.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.DateOfBirth), "Date of birth cannot be in the future.");
                return View(model);
            }

            var nationalId = NormalizeNationalId(model.NationalId);

            // Check National ID uniqueness
            if (nationalId != null)
            {
                bool nationalIdExists = _context.Patients.Any(p => p.NationalId == nationalId);
                if (nationalIdExists)
                {
                    ModelState.AddModelError(nameof(model.NationalId), "A patient with this National ID already exists.");
                    return View(model);
                }
            }

            // Create patient entity
            var patient = new Patient
            {
                FullName = model.FullName.Trim(),
                DateOfBirth = model.DateOfBirth,
                Gender = string.IsNullOrWhiteSpace(model.Gender) ? null : model.Gender.Trim(),
                NationalId = nationalId,
                Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                IsActive = model.IsActive
            };

            _context.Patients.Add(patient);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Handle duplicate National ID from DB
                if (IsUniqueConstraintViolation(ex))
                {
                    ModelState.AddModelError(nameof(model.NationalId), "A patient with this National ID already exists.");
                    return View(model);
                }

                throw;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Loads edit form for patient.
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!CanManagePatients())
                return RedirectToAction("AccessDenied", "Auth");

            var patient = _context.Patients.Find(id);
            if (patient == null) return NotFound();

            var model = new EditPatientViewModel
            {
                Id = patient.Id,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                NationalId = patient.NationalId,
                Address = patient.Address,
                PhoneNumber = patient.PhoneNumber,
                IsActive = patient.IsActive
            };

            return View(model);
        }

        /// <summary>
        /// Handles patient update.
        /// </summary>
        [HttpPost]
        public IActionResult Edit(EditPatientViewModel model)
        {
            if (!CanManagePatients())
                return RedirectToAction("AccessDenied", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            // Prevent future date
            if (model.DateOfBirth.HasValue && model.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.DateOfBirth), "Date of birth cannot be in the future.");
                return View(model);
            }

            var patient = _context.Patients.Find(model.Id);
            if (patient == null) return NotFound();

            var nationalId = NormalizeNationalId(model.NationalId);

            // Ensure National ID is unique (excluding current record)
            if (nationalId != null)
            {
                bool nationalIdExists = _context.Patients.Any(p => p.NationalId == nationalId && p.Id != model.Id);
                if (nationalIdExists)
                {
                    ModelState.AddModelError(nameof(model.NationalId), "A patient with this National ID already exists.");
                    return View(model);
                }
            }

            // Update fields
            patient.FullName = model.FullName.Trim();
            patient.DateOfBirth = model.DateOfBirth;
            patient.Gender = string.IsNullOrWhiteSpace(model.Gender) ? null : model.Gender.Trim();
            patient.NationalId = nationalId;
            patient.Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim();
            patient.PhoneNumber = model.PhoneNumber.Trim();
            patient.IsActive = model.IsActive;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (IsUniqueConstraintViolation(ex))
                {
                    ModelState.AddModelError(nameof(model.NationalId), "A patient with this National ID already exists.");
                    return View(model);
                }

                throw;
            }

            return RedirectToAction("Index");
        }
    }
}