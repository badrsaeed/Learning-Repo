using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System.Net;

namespace ViralWave.Application.Middlewares
{
    public class IpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IpWhitelistMiddleware> _logger;
        private readonly List<string> _allowedOrigins;

        public IpWhitelistMiddleware(RequestDelegate next, ILogger<IpWhitelistMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _allowedOrigins = configuration.GetSection("AllowedOrigins").Get<List<string>>();
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var origin = context.Request.Headers["Origin"].ToString();

            if (string.IsNullOrWhiteSpace(origin))
            {
                origin = context.Request.Headers["Referer"].ToString();
                
                if (!string.IsNullOrWhiteSpace(origin))
                {
                    origin = origin.Substring(0, origin.Length - 1);
                }
            }

            _logger.LogInformation($"Request from origin: {origin}");

            if (_allowedOrigins.Contains(origin))
            {
                await _next(context);
                return;
            }


            _logger.LogWarning($"Forbidden request from origin: {origin}");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden");
        }
    }
}
