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

                await HandleExceptionAsync(context,ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            
            context.Response.ContentType = "application/json";

            int statusCode;
            object response;

            if (ex is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status403Forbidden;
                response = new { message = ex.Message };
            }
            else if (ex is KeyNotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                response = new { message = ex.Message };
            }
            else if (ex is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                response = new { message = ex.Message };
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                response = new { message = "An unexpected error occurred. Please try again later." };
            }

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}