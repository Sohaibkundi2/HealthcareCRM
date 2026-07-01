using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using HealthcareCRM.Services;

namespace HealthcareCRM.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public AccountController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
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
            var user = _authService.GetUserByEmail(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // Verify password
            bool isPasswordValid = _authService.VerifyPassword(model.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // Generate JWT token and store in cookie
            var token = _authService.GenerateJwtToken(user);
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return RedirectToAction("Index", "Home");
        }
    }
}