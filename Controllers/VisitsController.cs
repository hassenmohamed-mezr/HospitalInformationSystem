using HospitalInformationSystem.Data;
using HospitalInformationSystem.Models;
using HospitalInformationSystem.ViewModels.CRUD_VisitViewMode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Controllers
{
    public class VisitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole()
        {
            return HttpContext.Session.GetString("Role");
        }

        private int? GetCurrentUserId()
        {
            var userIdRaw = HttpContext.Session.GetString("UserId");
            if (int.TryParse(userIdRaw, out var userId))
                return userId;

            return null;
        }

        private int? GetCurrentDoctorProfileId()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return null;

            var doctorProfile = _context.DoctorProfiles
                .AsNoTracking()
                .FirstOrDefault(dp => dp.UserId == userId.Value);

            return doctorProfile?.Id;
        }

        private void LoadCreateDropdowns()
        {
            var patients = _context.Patients
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.FullName)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FullName
                })
                .ToList();

            var doctors = _context.DoctorProfiles
                .AsNoTracking()
                .Include(dp => dp.User)
                .Where(dp => dp.IsActive && dp.User.IsActive)
                .OrderBy(dp => dp.User.FullName)
                .Select(dp => new SelectListItem
                {
                    Value = dp.Id.ToString(),
                    Text = $"{dp.User.FullName} ({dp.Specialty})"
                })
                .ToList();

            ViewBag.Patients = patients;
            ViewBag.Doctors = doctors;
        }

        public IActionResult Index()
        {
            var role = GetRole();
            if (role != "Admin" && role != "Reception" && role != "Doctor")
                return RedirectToAction("AccessDenied", "Auth");

            var query = _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.DoctorProfile)
                .ThenInclude(dp => dp.User)
                .AsQueryable();

            if (role == "Doctor")
            {
                var doctorProfileId = GetCurrentDoctorProfileId();
                if (!doctorProfileId.HasValue)
                    return RedirectToAction("AccessDenied", "Auth");

                query = query.Where(v => v.DoctorProfileId == doctorProfileId.Value);
            }

            var visits = query
                .OrderByDescending(v => v.VisitDate)
                .ToList();

            return View(visits);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (GetRole() != "Reception")
                return RedirectToAction("AccessDenied", "Auth");

            LoadCreateDropdowns();
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateVisitViewModel model)
        {
            if (GetRole() != "Reception")
                return RedirectToAction("AccessDenied", "Auth");

            LoadCreateDropdowns();

            if (!ModelState.IsValid)
                return View(model);

            var patientExists = _context.Patients.Any(p => p.Id == model.PatientId);
            if (!patientExists)
            {
                ModelState.AddModelError(nameof(model.PatientId), "Please select a valid patient.");
                return View(model);
            }

            var doctorExists = _context.DoctorProfiles.Any(dp => dp.Id == model.DoctorProfileId);
            if (!doctorExists)
            {
                ModelState.AddModelError(nameof(model.DoctorProfileId), "Please select a valid doctor.");
                return View(model);
            }

            var visit = new Visit
            {
                PatientId = model.PatientId,
                DoctorProfileId = model.DoctorProfileId,
                VisitDate = model.VisitDate,
                Reason = string.IsNullOrWhiteSpace(model.Reason) ? null : model.Reason.Trim(),
                Notes = null,
                Status = VisitStatus.Pending,
                CreatedAt = DateTime.Now
            };

            _context.Visits.Add(visit);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (GetRole() != "Doctor")
                return RedirectToAction("AccessDenied", "Auth");

            var doctorProfileId = GetCurrentDoctorProfileId();
            if (!doctorProfileId.HasValue)
                return RedirectToAction("AccessDenied", "Auth");

            var visit = _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.DoctorProfile)
                .ThenInclude(dp => dp.User)
                .FirstOrDefault(v => v.Id == id && v.DoctorProfileId == doctorProfileId.Value);

            if (visit == null)
                return NotFound();

            ViewBag.VisitInfo = $"{visit.Patient.FullName} - {visit.DoctorProfile.User.FullName} - {visit.VisitDate:yyyy-MM-dd HH:mm}";

            var model = new EditVisitViewModel
            {
                Id = visit.Id,
                Notes = visit.Notes,
                Status = visit.Status
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditVisitViewModel model)
        {
            if (GetRole() != "Doctor")
                return RedirectToAction("AccessDenied", "Auth");

            var doctorProfileId = GetCurrentDoctorProfileId();
            if (!doctorProfileId.HasValue)
                return RedirectToAction("AccessDenied", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var visit = _context.Visits
                .FirstOrDefault(v => v.Id == model.Id && v.DoctorProfileId == doctorProfileId.Value);

            if (visit == null)
                return NotFound();

            visit.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            visit.Status = model.Status;
            visit.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var role = GetRole();
            if (role != "Admin" && role != "Reception" && role != "Doctor")
                return RedirectToAction("AccessDenied", "Auth");

            var query = _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.DoctorProfile)
                .ThenInclude(dp => dp.User)
                .AsQueryable();

            if (role == "Doctor")
            {
                var doctorProfileId = GetCurrentDoctorProfileId();
                if (!doctorProfileId.HasValue)
                    return RedirectToAction("AccessDenied", "Auth");

                query = query.Where(v => v.DoctorProfileId == doctorProfileId.Value);
            }

            var visit = query.FirstOrDefault(v => v.Id == id);
            if (visit == null)
                return NotFound();

            return View(visit);
        }
    }
}
