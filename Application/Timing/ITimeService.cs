namespace Contracts.Timing
{
    public interface ITimeService
    {
        Task<T> MeasureTimeAsync<T>(Func<Task<T>> func, Action<TimeSpan> callback);
    }
}