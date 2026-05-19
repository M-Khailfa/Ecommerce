using System.Net;
using System.Text.Json;
using Ecommerce.Core.Settings;

namespace Ecommerce.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var message = _env.IsDevelopment() ? ex.Message : "Internal Server Error";
                var details = _env.IsDevelopment() ? ex.StackTrace : null;

                var response = ApiResponse.BadRequest(message);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(
                    _env.IsDevelopment()
                        ? new { statusCode = context.Response.StatusCode, message = ex.Message, details = ex.StackTrace }
                        : new { statusCode = context.Response.StatusCode, message = "Internal Server Error", details = (string?)null },
                    options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
