using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using System.Diagnostics;

namespace HealthcareCRM.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index
        public IActionResult Index()
        {
            // Check if JWT exists in request header
            var token = Request.Cookies["jwt"] ??
                        Request.Headers["Authorization"]
                        .ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}