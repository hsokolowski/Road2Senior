using System.Diagnostics;

namespace Infrastructure.Timing
{
    public class TimeService : ITimeService
    {
        public async Task<T> MeasureTimeAsync<T>(Func<Task<T>> func, Action<TimeSpan> callback)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await func();
            stopwatch.Stop();
            callback(stopwatch.Elapsed);
            return result;
        }
    }
}