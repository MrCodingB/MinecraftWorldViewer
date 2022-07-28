using System.Diagnostics;
using Minecraft.Regions;
using Minecraft.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utility;

namespace Minecraft;

public class RegionDrawer
{
    private Stopwatch ChunkStopwatch { get; } = new();

    private Stopwatch RegionStopwatch { get; } = new();

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
        => TrackRegionProgress(() => DrawChunks(region.ChunkHeaders, region.ReadFileAndDispose()));

    private void DrawChunks(IEnumerable<ChunkHeader> chunkHeaders, byte[] bytes)
    {
        foreach (var header in chunkHeaders)
        {
            TrackChunkProgress(() => DrawChunk(bytes, header.Offset));
        }
    }

    private void DrawChunk(byte[] bytes, int offset)
    {
        var length = BitHelper.ToInt32(bytes, offset);
        var compression = bytes[offset + 4];

        Chunk
            .FromBytes(compression, bytes, offset + 5, length - 1)
            .DrawChunk(Map, XRoot, ZRoot);
    }

    private void TrackRegionProgress(Action action)
        => ProgressManager.CompletedRegion(Profiler.MeasureExecutionTime(action, RegionStopwatch));

    private void TrackChunkProgress(Action action)
        => ProgressManager.CompletedChunk(Profiler.MeasureExecutionTime(action, ChunkStopwatch));
}
