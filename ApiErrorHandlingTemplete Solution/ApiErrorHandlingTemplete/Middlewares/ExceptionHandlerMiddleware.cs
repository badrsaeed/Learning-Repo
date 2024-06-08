using ApiErrorHandlingTemplete.Errors;
using System.Text.Json;

namespace ApiErrorHandlingTemplete.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger,IHostEnvironment env )
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex) 
            {
                _logger.LogError("An Exception has occured");

                var statusCode = (int)StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;
                
                var response = _env.IsDevelopment()?
                    new ExceptionHandlerResponse(statusCode, ex.Message, ex.StackTrace.ToString()):
                    new ExceptionHandlerResponse(statusCode);

                var option = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, option);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
