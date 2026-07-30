using System.Net;
using System.Text.Json;

namespace HealthcareCRM.Helpers
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                KeyNotFoundException => new
                {
                    success = false,
                    message = exception.Message,
                    data = (object?)null,
                    statusCode = 404
                },
                UnauthorizedAccessException => new
                {
                    success = false,
                    message = "Unauthorized access",
                    data = (object?)null,
                    statusCode = 401
                },
                ArgumentException => new
                {
                    success = false,
                    message = exception.Message,
                    data = (object?)null,
                    statusCode = 400
                },
                _ => new
                {
                    success = false,
                    message = "An unexpected error occurred. Please try again.",
                    data = (object?)null,
                    statusCode = 500
                }
            };

            context.Response.StatusCode = response.statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    // Extension method for clean registration
    public static class GlobalErrorHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalErrorHandlerMiddleware>();
        }
    }
}