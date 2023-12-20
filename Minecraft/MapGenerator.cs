using Minecraft.Regions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Minecraft;

public sealed class MapGenerator
{
    private DirectoryInfo RegionsDirectory { get; }

    public MapGenerator(string regionsPath)
    {
        RegionsDirectory = new DirectoryInfo(regionsPath);
    }

    public Image<Rgba32> Generate()
    {
        var regionFiles = LoadRegionFiles(out var chunkCount);

        ProgressManager.Reset(chunkCount, regionFiles.Count);

        return DrawMap(regionFiles);
    }

    private static Image<Rgba32> DrawMap(List<RegionFile> regionFiles)
    {
        var (width, offsetX, height, offsetZ) = GetMapDimensions(regionFiles);

        var map = new Image<Rgba32>(width, height);

        var drawer = new RegionDrawer(map, offsetX, offsetZ);

        foreach (var regionFile in regionFiles)
        {
            drawer.DrawAndDisposeRegion(regionFile);
        }

        return map;
    }

    private List<RegionFile> LoadRegionFiles(out int chunkCount)
    {
        chunkCount = 0;
        var maxFileSize = 0L;

        var files = RegionsDirectory.EnumerateFiles("r.*.*.mca", SearchOption.TopDirectoryOnly);

        var regionFiles = new List<RegionFile>();

        foreach (var file in files)
        {
            var regionFile = RegionFile.TryLoad(file.FullName);
            if (regionFile is null)
            {
                continue;
            }

            regionFiles.Add(regionFile);
            chunkCount += regionFile.ChunkCount;

            if (maxFileSize < file.Length)
            {
                maxFileSize = file.Length;
            }
        }

        RegionFile.SetMaxFileSize(maxFileSize);
        return regionFiles;
    }

    private static (int width, int offsetX, int height, int offsetZ) GetMapDimensions(IEnumerable<RegionFile> regions)
    {
        var minX = int.MaxValue;
        var maxX = int.MinValue;
        var minZ = int.MaxValue;
        var maxZ = int.MinValue;

        foreach (var regionFile in regions)
        {
            if (minX > regionFile.X) minX = regionFile.X;
            if (maxX < regionFile.X) maxX = regionFile.X;
            if (minZ > regionFile.Z) minZ = regionFile.Z;
            if (maxZ < regionFile.Z) maxZ = regionFile.Z;
        }

        return (
            ToAbsoluteCoordinate(maxX - minX + 1),
            ToAbsoluteCoordinate(minX),
            ToAbsoluteCoordinate(maxZ - minZ + 1),
            ToAbsoluteCoordinate(minZ)
        );
    }

    private static int ToAbsoluteCoordinate(int regionCoordinate) => regionCoordinate * 32 * 16;
}
