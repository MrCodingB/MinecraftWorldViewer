namespace Minecraft;

public static class ProgressManager
{
    public static event EventHandler<ProgressEventArgs>? Progress;

    private static int ChunkCount { get; set; }

    private static int CompletedChunksCount { get; set; }

    private static int RegionCount { get; set; }

    private static int CompletedRegionsCount { get; set; }

    public static void Reset() => Reset(0, 0);

    public static void Reset(int chunks, int regions)
    {
        ChunkCount = chunks;
        CompletedChunksCount = 0;
        RegionCount = regions;
        CompletedRegionsCount = 0;
        OnProgress();
    }

    public static void CompletedRegion()
    {
        CompletedRegionsCount++;
        OnProgress();
    }

    public static void CompletedChunk()
    {
        CompletedChunksCount++;
        OnProgress();
    }

    private static void OnProgress()
    {
        Progress?.Invoke(null,
            new ProgressEventArgs(ChunkCount, CompletedChunksCount, RegionCount, CompletedRegionsCount));
    }
}
