using HospitalInformationSystem.Data;
using HospitalInformationSystem.Models;
using HospitalInformationSystem.ViewModels.CRUD_AppointmentViewMode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Controllers
{
    /// <summary>
    /// Controller for managing appointments in the hospital management system.
    /// Reception/Admin create/edit, Doctors confirm/cancel/complete, all view with role-based filtering.
    /// Ensures no overlapping appointments and working hours compliance.
    /// </summary>
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole() => HttpContext.Session.GetString("Role");
        private int? GetCurrentUserId() => int.TryParse(HttpContext.Session.GetString("UserId"), out var id) ? id : null;
        private int? GetCurrentDoctorProfileId()
        {
            var userId = GetCurrentUserId();
            return userId.HasValue ? _context.DoctorProfiles.AsNoTracking().FirstOrDefault(dp => dp.UserId == userId)?.Id : null;
        }

        private bool CanManageAppointments() => GetRole() is "Admin" or "Reception";
        private bool CanViewAppointments() => GetRole() is "Admin" or "Reception" or "Doctor";

        private void LoadDropdowns()
        {
            var patients = _context.Patients.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.FullName)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FullName }).ToList();
            var doctors = _context.DoctorProfiles.AsNoTracking().Include(dp => dp.User).Where(dp => dp.IsActive && dp.User.IsActive)
                .OrderBy(dp => dp.User.FullName).Select(dp => new SelectListItem { Value = dp.Id.ToString(), Text = $"{dp.User.FullName} ({dp.Specialty})" }).ToList();
            ViewBag.Patients = patients;
            ViewBag.Doctors = doctors;
        }

        private bool IsWorkingHours(DateTime dateTime) => dateTime.Hour >= 9 && dateTime.Hour < 17;

        private bool HasOverlap(int doctorProfileId, DateTime newStart, int duration, int? excludeAppointmentId = null)
        {
            var newEnd = newStart.AddMinutes(duration);
            return _context.Appointments.AsNoTracking()
                .Where(a => a.DoctorProfileId == doctorProfileId && a.Id != excludeAppointmentId && a.Status != AppointmentStatus.Cancelled)
                .Any(a => a.AppointmentDateTime < newEnd && a.AppointmentDateTime.AddMinutes(a.Duration ?? 30) > newStart);
        }

        public IActionResult Index(string? status, DateTime? dateFrom, DateTime? dateTo)
        {
            if (!CanViewAppointments()) return RedirectToAction("AccessDenied", "Auth");

            var query = _context.Appointments.Include(a => a.Patient).Include(a => a.DoctorProfile).ThenInclude(dp => dp.User).AsQueryable();

            if (GetRole() == "Doctor")
            {
                var doctorId = GetCurrentDoctorProfileId();
                if (!doctorId.HasValue) return RedirectToAction("AccessDenied", "Auth");
                query = query.Where(a => a.DoctorProfileId == doctorId);
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<AppointmentStatus>(status, out var stat))
                query = query.Where(a => a.Status == stat);

            if (dateFrom.HasValue) query = query.Where(a => a.AppointmentDateTime >= dateFrom);
            if (dateTo.HasValue) query = query.Where(a => a.AppointmentDateTime <= dateTo);

            ViewBag.Status = status;
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");

            var appointments = query.OrderByDescending(a => a.AppointmentDateTime).ToList();
            return View(appointments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!CanManageAppointments()) return RedirectToAction("AccessDenied", "Auth");
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateAppointmentViewModel model)
        {
            if (!CanManageAppointments()) return RedirectToAction("AccessDenied", "Auth");

            LoadDropdowns();

            if (!ModelState.IsValid) return View(model);

            if (model.AppointmentDateTime <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(model.AppointmentDateTime), "Appointment date must be in the future.");
                return View(model);
            }

            if (!IsWorkingHours(model.AppointmentDateTime))
            {
                ModelState.AddModelError(nameof(model.AppointmentDateTime), "Appointments are only allowed between 9 AM and 5 PM.");
                return View(model);
            }

            if (HasOverlap(model.DoctorProfileId, model.AppointmentDateTime, model.Duration))
            {
                ModelState.AddModelError("", "This appointment overlaps with an existing one for the selected doctor.");
                return View(model);
            }

            var appointment = new Appointment
            {
                PatientId = model.PatientId,
                DoctorProfileId = model.DoctorProfileId,
                AppointmentDateTime = model.AppointmentDateTime,
                Duration = model.Duration,
                Notes = model.Notes
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!CanManageAppointments()) return RedirectToAction("AccessDenied", "Auth");

            var appointment = _context.Appointments.Include(a => a.Patient).Include(a => a.DoctorProfile).FirstOrDefault(a => a.Id == id);
            if (appointment == null) return NotFound();

            LoadDropdowns();

            var model = new EditAppointmentViewModel
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorProfileId = appointment.DoctorProfileId,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Duration = appointment.Duration ?? 30,
                Status = appointment.Status,
                Notes = appointment.Notes
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditAppointmentViewModel model)
        {
            if (!CanManageAppointments()) return RedirectToAction("AccessDenied", "Auth");

            LoadDropdowns();

            if (!ModelState.IsValid) return View(model);

            var appointment = _context.Appointments.Find(model.Id);
            if (appointment == null) return NotFound();

            if (model.AppointmentDateTime <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(model.AppointmentDateTime), "Appointment date must be in the future.");
                return View(model);
            }

            if (!IsWorkingHours(model.AppointmentDateTime))
            {
                ModelState.AddModelError(nameof(model.AppointmentDateTime), "Appointments are only allowed between 9 AM and 5 PM.");
                return View(model);
            }

            if (HasOverlap(model.DoctorProfileId, model.AppointmentDateTime, model.Duration, model.Id))
            {
                ModelState.AddModelError("", "This appointment overlaps with an existing one for the selected doctor.");
                return View(model);
            }

            appointment.PatientId = model.PatientId;
            appointment.DoctorProfileId = model.DoctorProfileId;
            appointment.AppointmentDateTime = model.AppointmentDateTime;
            appointment.Duration = model.Duration;
            appointment.Status = model.Status;
            appointment.Notes = model.Notes;
            appointment.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            if (!CanViewAppointments()) return RedirectToAction("AccessDenied", "Auth");

            var appointment = _context.Appointments.Include(a => a.Patient).Include(a => a.DoctorProfile).ThenInclude(dp => dp.User).FirstOrDefault(a => a.Id == id);
            if (appointment == null) return NotFound();

            if (GetRole() == "Doctor")
            {
                var doctorId = GetCurrentDoctorProfileId();
                if (appointment.DoctorProfileId != doctorId) return RedirectToAction("AccessDenied", "Auth");
            }

            return View(appointment);
        }

        [HttpPost]
        public IActionResult Confirm(int id)
        {
            var role = GetRole();
            if (role != "Doctor" && role != "Reception" && role != "Admin") return RedirectToAction("AccessDenied", "Auth");

            var appointment = _context.Appointments.Find(id);
            if (appointment == null) return NotFound();

            if (role == "Doctor")
            {
                var doctorId = GetCurrentDoctorProfileId();
                if (appointment.DoctorProfileId != doctorId) return RedirectToAction("AccessDenied", "Auth");
            }

            if (appointment.Status != AppointmentStatus.Pending) return BadRequest("Can only confirm pending appointments.");

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var role = GetRole();
            if (role != "Doctor" && role != "Reception" && role != "Admin") return RedirectToAction("AccessDenied", "Auth");

            var appointment = _context.Appointments.Find(id);
            if (appointment == null) return NotFound();

            if (role == "Doctor")
            {
                var doctorId = GetCurrentDoctorProfileId();
                if (appointment.DoctorProfileId != doctorId) return RedirectToAction("AccessDenied", "Auth");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Complete(int id)
        {
            var role = GetRole();
            if (role != "Doctor" && role != "Reception" && role != "Admin") return RedirectToAction("AccessDenied", "Auth");

            var appointment = _context.Appointments.Find(id);
            if (appointment == null) return NotFound();

            if (role == "Doctor")
            {
                var doctorId = GetCurrentDoctorProfileId();
                if (appointment.DoctorProfileId != doctorId) return RedirectToAction("AccessDenied", "Auth");
            }

            if (appointment.Status != AppointmentStatus.Confirmed) return BadRequest("Can only complete confirmed appointments.");

            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedAt = DateTime.Now;

            // Create Visit
            var visit = new Visit
            {
                PatientId = appointment.PatientId,
                DoctorProfileId = appointment.DoctorProfileId,
                VisitDate = appointment.AppointmentDateTime,
                Reason = $"Appointment completed - {appointment.Notes}",
                Notes = "Created from completed appointment.",
                Status = VisitStatus.Completed
            };
            _context.Visits.Add(visit);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!CanManageAppointments()) return RedirectToAction("AccessDenied", "Auth");

            var appointment = _context.Appointments.Find(id);
            if (appointment == null) return NotFound();

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}