namespace Contracts.Cache
{
    public interface ICacheService
    {
        Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan duration);
    }
}