using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan duration)
        {
            if (_cache.TryGetValue(key, out T value))
            {
                return value;
            }

            value = await factory();
            _cache.Set(key, value, duration);
            return value;
        }
    }
}