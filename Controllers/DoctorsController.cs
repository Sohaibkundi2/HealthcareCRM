using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.Controllers
{
    public class DoctorsController : Controller
    {
        // GET: /Doctors
        public IActionResult Index()
        {
            return View();
        }
    }
}