using Microsoft.Extensions.Caching.Memory;

namespace ViralWave.Infrastructure.Services;

public class CachingManagement
{
    private readonly IMemoryCache _cache;

    public CachingManagement(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task CacheDataAsync(string key, string data, TimeSpan? expirationTime = null)
    {
        // Assuming _cache is initialized using an injected IMemoryCache instance (e.g., MemoryCache).
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromHours(1)
        };

        _cache.Set(key, data, cacheOptions);
    }
    public async Task<string?> GetCachedDataAsync(string key)
    {
        // Assuming _cache is an instance of IMemoryCache
        if (_cache.TryGetValue(key, out string cachedData))
        {
            // Return cached data if it exists
            return await Task.FromResult(cachedData);
        }

        // If not present, return null
        return null;
    }
}