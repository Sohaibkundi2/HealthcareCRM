using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using HealthcareCRM.Services;

namespace HealthcareCRM.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthApiController(AuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input" });

            var user = _authService.GetUserByEmail(model.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            bool isPasswordValid = _authService.VerifyPassword(model.Password, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _authService.GenerateJwtToken(user);

            return Ok(new { token, message = "Login successful" });
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input" });

            bool success = await _authService.RegisterUser(model);

            if (!success)
                return BadRequest(new { message = "Email already exists" });

            var user = _authService.GetUserByEmail(model.Email);
            var token = _authService.GenerateJwtToken(user!);

            return Ok(new { token, message = "Registration successful" });
        }
    }
}