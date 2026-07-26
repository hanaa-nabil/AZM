using System.Net;
using System.Text.Json;

namespace AZM.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);

                var (statusCode, message) = ex switch
                {
                    KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
                    UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ex.Message),
                    InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
                    _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var json = JsonSerializer.Serialize(new { error = message });
                await context.Response.WriteAsync(json);
            }
        }
    }
}