using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.Controllers
{
    public class AppointmentsController : Controller
    {
        // GET: /Appointments
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Appointments/Book
        public IActionResult Book()
        {
            return View();
        }
    }
}