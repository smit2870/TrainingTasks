using System.Net;
using System.Text.Json;

namespace taskmanagement.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            
            context.Response.ContentType = "application/json";

            int statusCode = context.Response.StatusCode;

            if (statusCode == 0)
                statusCode = StatusCodes.Status500InternalServerError;

            var response = statusCode switch
                {
                    400 => new { message = "Bad request." },
                    401 => new { message = "Unauthorized." },
                    404 => new { message = "Resource not found." },
                    _ => new { message = "An unexpected error occurred. Please try again later." }
                };
            
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}