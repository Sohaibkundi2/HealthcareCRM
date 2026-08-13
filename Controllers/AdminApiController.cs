using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Models;
using HealthcareCRM.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        private string GetCurrentUserEmail()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader)) return "Unknown";
            var token = authHeader.Substring("Bearer ".Length);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unknown";
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
                    u.IsActive,
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

            var oldRole = user.Role;
            user.Role = request.Role;

            // Atomic — save role change and audit log together
            _context.AuditLogs.Add(new AuditLog
            {
                Action = "ROLE_CHANGE",
                TargetType = "User",
                TargetId = id,
                PerformedBy = GetCurrentUserEmail(),
                Details = $"Role changed from {oldRole} to {request.Role} for {user.Email}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Role updated successfully" });
        }

        // PUT /api/admin/users/{id}/toggle-active
        [HttpPut("users/{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            user.IsActive = !user.IsActive;

            // Atomic — save status change and audit log together
            _context.AuditLogs.Add(new AuditLog
            {
                Action = user.IsActive ? "USER_ACTIVATED" : "USER_DEACTIVATED",
                TargetType = "User",
                TargetId = id,
                PerformedBy = GetCurrentUserEmail(),
                Details = $"{user.Email} was {(user.IsActive ? "activated" : "deactivated")}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = user.IsActive ? "User activated successfully" : "User deactivated successfully",
                isActive = user.IsActive
            });
        }

        // GET /api/admin/audit-log
        [HttpGet("audit-log")]
        public IActionResult GetAuditLog()
        {
            var logs = _context.AuditLogs
                .OrderByDescending(a => a.CreatedAt)
                .Take(100)
                .Select(a => new
                {
                    a.Id,
                    a.Action,
                    a.TargetType,
                    a.TargetId,
                    a.PerformedBy,
                    a.Details,
                    a.CreatedAt
                })
                .ToList();

            return Ok(new { success = true, data = logs });
        }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}