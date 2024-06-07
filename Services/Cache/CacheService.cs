using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Services.Timing;

namespace Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly ITimeService _timeService;

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger, ITimeService timeService)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _timeService = timeService;
        }

        public async Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan duration)
        {
            if (_memoryCache.TryGetValue(cacheKey, out T cacheEntry))
            {
                var cachedData = await _timeService.MeasureTimeAsync(async () => cacheEntry, elapsed =>
                {
                    _logger.LogInformation($"Data retrieved from cache in {elapsed.TotalMilliseconds} ms");
                });
                return cachedData;
            }

            var data = await _timeService.MeasureTimeAsync(factory, elapsed =>
            {
                _logger.LogInformation($"Data retrieved from API in {elapsed.TotalMilliseconds} ms");
            });

            _memoryCache.Set(cacheKey, data, duration);
            return data;
        }
    }
}