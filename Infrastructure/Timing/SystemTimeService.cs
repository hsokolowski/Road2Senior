using System.Diagnostics;
using Contracts.Timing;

namespace Infrastructure.Timing
{
    public class SystemTimeService : ITimeService
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