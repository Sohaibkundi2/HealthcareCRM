using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.Controllers
{
    public class PatientsController : Controller
    {
        // GET: /Patients
        public IActionResult Index()
        {
            return View();
        }
    }
}