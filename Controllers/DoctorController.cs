using Microsoft.AspNetCore.Mvc;

namespace HospitalInformationSystem.Controllers
{

    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Doctor")
                return RedirectToAction("AccessDenied", "Auth");
            return View();
        }
    }
}
