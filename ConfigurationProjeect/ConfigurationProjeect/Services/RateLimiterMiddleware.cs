using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ViralWave.Application.Middlewares;
public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _memoryCache;
    private readonly int _maxRequestsPerSecond;
    private readonly int _timeInSeconds;
    private readonly ILogger<RateLimiterMiddleware> _logger;
    
    public RateLimiterMiddleware(
        RequestDelegate next, 
        IMemoryCache memoryCache, 
        IConfiguration configuration,
        ILogger<RateLimiterMiddleware> logger)
    {
        _next = next;
        _memoryCache = memoryCache;
        _logger = logger;
        _maxRequestsPerSecond = configuration.GetValue<int>("RateLimiter:RequestLimit");
        _timeInSeconds = configuration.GetValue<int>("RateLimiter:TimeInSeconds");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        
        if (clientIp == null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            _logger.LogError("Client IP address was not provided.");
            return;
        }

        try
        {
           
            var cacheKey = $"RateLimit_{clientIp}";
            var requests = _memoryCache.Get<int>(cacheKey);

            if (requests >= _maxRequestsPerSecond)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests.");
                _logger.LogError($"Too many requests for IP address {clientIp}.");
                return;
            }

            _memoryCache.Set(cacheKey, requests + 1, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_timeInSeconds) 
            });

            await _next(context);
        }
        catch(Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
        }
    }
}