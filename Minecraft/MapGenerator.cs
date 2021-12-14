using System.Text.RegularExpressions;
using Minecraft.Regions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Minecraft;

public class MapGenerator
{
    private DirectoryInfo RegionsDirectory { get; }

    public MapGenerator(string regionsPath)
    {
        RegionsDirectory = new DirectoryInfo(regionsPath);
    }

    public Image<Rgba32> Generate()
    {
        var files = RegionsDirectory
            .EnumerateFiles("r.*.*.mca", SearchOption.TopDirectoryOnly)
            .Where(f => Regex.IsMatch(f.Name, @"r\.\-?\d+\.\-?\d+\.mca"));

        var regionFiles = files.Select(f => RegionFile.Load(f.FullName)).ToArray();

        var regionCount = regionFiles.Length;
        var chunkCount = regionFiles.Sum(f => f.ChunkCount);

        ProgressManager.Reset(chunkCount, regionCount);

        var (width, offsetX, height, offsetZ) = GetMapDimensions(regionFiles);

        var map = new Image<Rgba32>(width, height);

        map.Mutate(mapBitmap =>
        {
            foreach (var regionFile in regionFiles)
            {
                var x = regionFile.X * 32 * 16;
                var z = regionFile.Z * 32 * 16;

                using var regionBitmap = regionFile.CalculateBitmap();

                mapBitmap.DrawImage(regionBitmap, new Point(x - offsetX, z - offsetZ), 1f);

                regionFile.Dispose();

                ProgressManager.CompletedRegion();
            }
        });

        return map;
    }

    private static (int width, int offsetX, int height, int offsetZ) GetMapDimensions(RegionFile[] regionFiles)
    {
        var minX = ToAbsoluteCoordinates(regionFiles.Min(r => r.X) - 1);
        var maxX = ToAbsoluteCoordinates(regionFiles.Max(r => r.X) + 1);
        var minZ = ToAbsoluteCoordinates(regionFiles.Min(r => r.Z) - 1);
        var maxZ = ToAbsoluteCoordinates(regionFiles.Max(r => r.Z) + 1);

        return (maxX - minX, minX, maxZ - minZ, minZ);
    }

    private static int ToAbsoluteCoordinates(int regionCoordinate)
        => regionCoordinate * 32 * 16;
}
