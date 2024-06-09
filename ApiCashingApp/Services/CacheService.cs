
using System.Runtime.Caching;

namespace ApiCashingApp.Services;

public class CacheService : ICacheService
{
    private ObjectCache _memoryCache = MemoryCache.Default;

    public CacheService()
    {
    }
    public T GetData<T>(string key)
    {
        try
        {
            if(!string.IsNullOrEmpty(key))
            {
                T item = (T) _memoryCache.Get(key);
                return item;
            }
            return default;
        }catch(Exception ex)
        {
            throw;
        }
    }

    public object RemoveData(string key)
    {
       try
       {
         if(!string.IsNullOrEmpty(key))
        {
            _memoryCache.Remove(key);
            return true;
        }
        return false;
       }catch(Exception ex)
       {
        throw;
       }
    }

    public bool SetData<T>(string key, T? value, DateTimeOffset expirationDateTime)
    {
        try
        {
            if(!string.IsNullOrEmpty(key))
            {
                _memoryCache.Set(key, value, expirationDateTime);
                return true;
            }
            return false;
        }catch(Exception ex)
        {
            throw;
        }
    }
}