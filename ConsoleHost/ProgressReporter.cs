using Minecraft;

namespace ConsoleHost;

public static class ProgressReporter
{
    private static readonly int MaxColumns = Console.WindowWidth;

    private static bool FirstMessage = true;

    private static int CurrentColumns;

    public static void ReportProgress(object? sender, ProgressEventArgs e)
    {
        if (FirstMessage)
        {
            Console.WriteLine("Discovered {0} regions and {1} chunks", e.TotalRegions, e.TotalChunks);
            FirstMessage = false;
        }

        if (e.TotalChunks == 0)
        {
            return;
        }

        var newColumns = (int)Math.Ceiling((decimal)e.CompletedChunks / e.TotalChunks * MaxColumns);

        var currentColumns = Interlocked.Exchange(ref CurrentColumns, newColumns);

        var columnsToAdd = newColumns - currentColumns;

        if (columnsToAdd > 0)
        {
            Console.Write(new string('#', columnsToAdd));
        }
    }
}
