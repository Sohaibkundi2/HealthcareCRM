using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HealthcareCRM.Helpers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RequireRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    success = false,
                    message = "Authentication required"
                });
                return;
            }

            var token = authHeader.Substring("Bearer ".Length);

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var role = jwt.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role == null || !_roles.Contains(role))
                {
                    context.Result = new ObjectResult(new
                    {
                        success = false,
                        message = "Access denied — insufficient permissions"
                    })
                    { StatusCode = 403 };
                    return;
                }
            }
            catch
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    success = false,
                    message = "Invalid or expired token"
                });
            }
        }
    }
}