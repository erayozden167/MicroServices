using CartAPI.Business.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace CartAPI.Business.Services
{
    public class GenericCacheService : IGenericCacheService // Geliştirme aşaması.
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _defaultOptions;

        public GenericCacheService(IDistributedCache cache)
        {
            _cache = cache;
            _defaultOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
            };
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            var cachedValue = await _cache.GetStringAsync(key);
            return string.IsNullOrEmpty(cachedValue) ? null : JsonSerializer.Deserialize<T>(cachedValue);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null) where T : class
        {
            var options = expirationTime.HasValue
                ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime.Value }
                : _defaultOptions;

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _cache.GetStringAsync(key) != null;
        }
    }
}
