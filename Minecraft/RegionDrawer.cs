using System.Diagnostics;
using Minecraft.Regions;
using Minecraft.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utility;

namespace Minecraft;

public sealed class RegionDrawer
{
    private static Stopwatch RegionStopwatch { get; } = new();

    private Image<Rgba32> Map { get; }

    private int XRoot { get; }

    private int ZRoot { get; }

    public RegionDrawer(Image<Rgba32> map, int xRoot, int zRoot)
    {
        Map = map;
        XRoot = xRoot;
        ZRoot = zRoot;
    }

    public void DrawAndDisposeRegion(RegionFile region)
        => TrackRegionProgress(() => DrawChunksParallel(region.ChunkStartOffsets, region.ReadFileAndDispose()));

    private void DrawChunksParallel(IEnumerable<int> chunkStartOffsets, byte[] bytes)
        => Parallel.ForEach(chunkStartOffsets, offset => TrackChunkProgress(() => DrawChunk(bytes, offset)));

    private void DrawChunks(IEnumerable<int> chunkStartOffsets, byte[] bytes)
    {
        foreach (var offset in chunkStartOffsets)
        {
            TrackChunkProgress(() => DrawChunk(bytes, offset));
        }
    }

    private void DrawChunk(byte[] bytes, int offset)
    {
        var length = BitHelper.ToInt32(bytes, offset);
        var compression = bytes[offset + 4];

        Chunk
            .FromBytes(compression, bytes, offset + 5, length - 1)
            ?.DrawChunk(Map, XRoot, ZRoot);
    }

    private static void TrackRegionProgress(Action action)
        => ProgressManager.CompletedRegion(Profiler.MeasureExecutionTime(action, RegionStopwatch));

    private static void TrackChunkProgress(Action action)
        => ProgressManager.CompletedChunk(Profiler.MeasureExecutionTime(action));
}
