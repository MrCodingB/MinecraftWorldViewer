using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Minecraft.Regions;

public class RegionFile : IDisposable
{
    public int X { get; }

    public int Z { get; }

    public int ChunkCount => ChunkPositions.Count;

    private IList<int> ChunkPositions { get; }

    private FileStream FileStream { get; }

    private RegionFile(int x, int z, IList<int> chunkPositions, FileStream fileStream)
    {
        X = x;
        Z = z;
        ChunkPositions = chunkPositions;
        FileStream = fileStream;
    }

    public static RegionFile Load(string path)
    {
        var matches = Regex.Match(path, @"r\.(\-?\d+)\.(\-?\d+)\.mca$");

        if (!matches.Success)
        {
            throw new ArgumentException("Invalid region file path: " + path, nameof(path));
        }

        var x = int.Parse(matches.Groups[1].Value);
        var z = int.Parse(matches.Groups[2].Value);

        var regionFileStream = File.OpenRead(path);

        var chunks = GetChunkHeaders(regionFileStream);

        return new RegionFile(x, z, chunks, regionFileStream);
    }

    public Image<Rgba32> CalculateBitmap()
    {
        var regionX = X * 32;
        var regionZ = Z * 32;

        var regionBitmap = new Image<Rgba32>(32 * 16, 32 * 16);

        regionBitmap.Mutate(region =>
        {
            foreach (var chunkPosition in ChunkPositions)
            {
                FileStream.Seek(chunkPosition, SeekOrigin.Begin);

                var length = FileStream.GetInt32();

                var chunkBytes = FileStream.GetBytes(length);

                var chunk = Chunk.FromStream(new MemoryStream(chunkBytes));

                var x = chunk.X - regionX;
                var z = chunk.Z - regionZ;

                using var chunkBitmap = chunk.CalculateBitmap();

                region.DrawImage(chunkBitmap, new Point(x * 16, z * 16), 1f);
            }
        });

        return regionBitmap;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FileStream.Dispose();
    }

    private static IList<int> GetChunkHeaders(Stream stream)
    {
        var chunks = new List<int>(1024);

        // Header size is 8kB, last 4kB are timestamps => irrelevant
        var bytes = stream.GetBytes(4096);
        stream.Seek(4096, SeekOrigin.Current);

        var i = 0;

        while (i < 4096)
        {
            var position = BitHelper.ToInt24(bytes, i) * 4096;
            if (position > 1) // If chunk isn't loaded, position and length are 0
            {
                chunks.Add(position);
            }

            i += 4;
        }

        chunks.Sort();

        return chunks;
    }
}
