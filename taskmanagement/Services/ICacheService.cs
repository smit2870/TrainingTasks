namespace taskmanagement.Services
{
    
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T data, TimeSpan? ttl = null);
        Task RemoveAsync(string key);
    }

}