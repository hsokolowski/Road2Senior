using System;
using System.Threading.Tasks;

namespace Services.Timing
{
    public interface ITimeService
    {
        Task<T> MeasureTimeAsync<T>(Func<Task<T>> func, Action<TimeSpan> callback);
    }
}