using System.Diagnostics;

namespace Minecraft;

public static class ProgressManager
{
    public static event EventHandler<ProgressEventArgs>? Progress;

    public static decimal MaxMemory { get; private set; }

    public static TimeSpan TimePerChunk => CompletedChunksCount > 0
        ? TotalChunkTime / CompletedChunksCount
        : TimeSpan.Zero;

    public static TimeSpan TimePerRegion => CompletedRegionsCount > 0
        ? TotalRegionTime / CompletedRegionsCount
        : TimeSpan.Zero;

    public static decimal AvgMemoryInMb => MemoryReadings > 0
        ? TotalMemoryInMb / MemoryReadings
        : 0;

    private static readonly Process CurrentProcess = Process.GetCurrentProcess();

    private static int ChunkCount { get; set; }

    private static int CompletedChunksCount { get; set; }

    private static TimeSpan TotalChunkTime { get; set; }

    private static int RegionCount { get; set; }

    private static int CompletedRegionsCount { get; set; }

    private static TimeSpan TotalRegionTime { get; set; }

    private static decimal TotalMemoryInMb { get; set; }

    private static int MemoryReadings { get; set; }

    public static void Reset() => Reset(0, 0);

    public static void Reset(int chunks, int regions)
    {
        ChunkCount = chunks;
        CompletedChunksCount = 0;
        RegionCount = regions;
        CompletedRegionsCount = 0;
        MemoryReadings = 0;
        TotalMemoryInMb = 0;
        OnProgress();
    }

    public static void CompletedRegion(TimeSpan timeTaken)
    {
        CompletedRegionsCount++;
        TotalRegionTime += timeTaken;
        OnProgress();
    }

    public static void CompletedChunk(TimeSpan timeTaken)
    {
        CompletedChunksCount++;
        TotalChunkTime += timeTaken;
        OnProgress();
    }

    private static void OnProgress()
    {
        var peakMemory = (decimal)CurrentProcess.WorkingSet64 / 1024 / 1024;
        TotalMemoryInMb += peakMemory;
        MemoryReadings++;
        if (MaxMemory < peakMemory)
        {
            MaxMemory = peakMemory;
        }

        Progress?.Invoke(null, new ProgressEventArgs(ChunkCount, CompletedChunksCount, RegionCount, CompletedRegionsCount));
    }
}
