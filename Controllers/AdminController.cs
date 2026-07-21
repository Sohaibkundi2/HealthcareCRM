using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Helpers;

namespace HealthcareCRM.Controllers
{
    public class AdminController : Controller
    {
        // GET: /Admin/Users
        public IActionResult Users()
        {
            return View();
        }
    }
}