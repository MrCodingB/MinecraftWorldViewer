namespace Minecraft;

public class ProgressEventArgs
{
    public int TotalChunks { get; }

    public int CompletedChunks { get; }

    public int TotalRegions { get; }

    public int CompletedRegions { get; }

    public ProgressEventArgs(int totalChunks, int completedChunks, int totalRegions, int completedRegions)
    {
        TotalChunks = totalChunks;
        CompletedChunks = completedChunks;
        TotalRegions = totalRegions;
        CompletedRegions = completedRegions;
    }
}
