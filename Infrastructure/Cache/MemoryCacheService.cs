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
    
    // public class MemoryCacheService : ICacheService
    // {
    //     private readonly IMemoryCache _memoryCache;
    //     private readonly ILogger<MemoryCacheService> _logger;
    //     private readonly ITimeService _timeService;
    //
    //     public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger, ITimeService timeService)
    //     {
    //         _memoryCache = memoryCache;
    //         _logger = logger;
    //         _timeService = timeService;
    //     }
    //
    //     public async Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan duration)
    //     {
    //         if (_memoryCache.TryGetValue(cacheKey, out T cacheEntry))
    //         {
    //             var cachedData = await _timeService.MeasureTimeAsync(async () => cacheEntry, elapsed =>
    //             {
    //                 _logger.LogInformation($"Data retrieved from cache in {elapsed.TotalMilliseconds} ms");
    //             });
    //             return cachedData;
    //         }
    //
    //         var data = await _timeService.MeasureTimeAsync(factory, elapsed =>
    //         {
    //             _logger.LogInformation($"Data retrieved from API in {elapsed.TotalMilliseconds} ms");
    //         });
    //
    //         _memoryCache.Set(cacheKey, data, duration);
    //         return data;
    //     }
    // }
}