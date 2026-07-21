using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using HealthcareCRM.Helpers;

namespace HealthcareCRM.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [RequireRole("Admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/admin/users
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                })
                .ToList();

            return Ok(new { success = true, data = users });
        }

        // PUT /api/admin/users/{id}
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            var validRoles = new[] { "Admin", "Staff" };
            if (!validRoles.Contains(request.Role))
                return BadRequest(new { success = false, message = "Invalid role" });

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            user.Role = request.Role;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Role updated successfully" });
        }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}