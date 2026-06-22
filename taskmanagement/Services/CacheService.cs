
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace taskmanagement.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var data = await _cache.GetStringAsync(key);

                if (data == null)
                    return default;

                _logger.LogInformation("Cache HIT: {Key}", key);

                return JsonSerializer.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache GET failed {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T data, TimeSpan? ttl = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromMinutes(5)
                };

                var json = JsonSerializer.Serialize(data);

                await _cache.SetStringAsync(key, json, options);

                _logger.LogInformation("Cache SET: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache SET failed {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogInformation("Cache REMOVED: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache REMOVE failed {Key}", key);
            }
        }
    }

}