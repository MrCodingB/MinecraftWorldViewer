using System.Diagnostics;

namespace Minecraft;

public static class Profiler
{
    public static void MeasureExecutionDuration(string methodName, Action function)
    {
        if (!Debugger.IsAttached)
        {
            function();
            return;
        }
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        function();

        stopwatch.Stop();
        Debugger.Log(0, methodName, $"{stopwatch.Elapsed}\n");
    }

    public static T MeasureExecutionDuration<T>(string methodName, Func<T> function)
    {
        if (!Debugger.IsAttached)
        {
            return function();
        }
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var result = function();

        stopwatch.Stop();
        Debugger.Log(0, methodName, $"{stopwatch.Elapsed}\n");

        return result;
    }
}
