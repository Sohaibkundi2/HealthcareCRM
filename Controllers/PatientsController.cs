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
        // GET: /Patients/Detail/{id}
        public IActionResult Detail(int id)
        {
            return View(model: id);
        }
    }
}