using System.Diagnostics;

namespace Utility;

public static class Profiler
{
    public static TimeSpan MeasureExecutionTime(Action action)
    {
        var stopwatch = Stopwatch.StartNew();

        action();

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    public static TimeSpan MeasureExecutionTime(Action action, Stopwatch stopwatch)
    {
        stopwatch.Restart();

        action();

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
}
