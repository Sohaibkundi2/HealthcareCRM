using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;

namespace HealthcareCRM.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Find user by email
            var user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // TODO: verify password hash + generate JWT (next step)

            return RedirectToAction("Index", "Home");
        }
    }
}