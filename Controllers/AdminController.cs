using Microsoft.AspNetCore.Mvc;

namespace HospitalInformationSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Auth");

            return View();
        }
    }
}