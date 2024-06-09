namespace ApiCashingApp.Services;
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T? value, DateTimeOffset expirationDateTime);
        object RemoveData(string key);
    }
