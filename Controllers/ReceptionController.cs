using Microsoft.AspNetCore.Mvc;

namespace HospitalInformationSystem.Controllers
{
    public class ReceptionController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Reception")
                return RedirectToAction("AccessDenied", "Auth");

            return View();
        }
    }
}