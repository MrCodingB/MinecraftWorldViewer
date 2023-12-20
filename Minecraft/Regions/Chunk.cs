using System.IO.Compression;
using Minecraft.NBT;
using Minecraft.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Minecraft.Regions;

public class Chunk
{
    private const byte GZipCompression = 1;
    private const byte ZlibCompression = 2;
    private const byte Uncompressed = 3;

    private int X { get; }

    private int Z { get; }

    private ChunkSection[] Sections { get; }

    private ushort[] Heightmap { get; }

    public Chunk(int x, int z, ChunkSection[] sections, ushort[] heightmap)
    {
        X = x;
        Z = z;
        Sections = sections;
        Heightmap = heightmap;
    }

    public static Chunk? FromBytes(int compression, byte[] bytes, int index, int count)
    {
        using var stream = GetValueStream(compression, bytes, index, count);

        var type = stream.GetTagType();
        var nameLength = stream.GetUInt16();

        if (type != TagType.Compound || nameLength != 0)
        {
            throw new InvalidOperationException("Invalid root tag for chunk");
        }

        return new ChunkParser().ParseChunk(stream);
    }

    public void DrawChunk(Image<Rgba32> mapImage, int x0, int z0)
    {
        var xOffset = X * 16 - x0;
        var zOffset = Z * 16 - z0;

        var zxOffset = 0;

        for (int chunkZ = 0, z = zOffset; chunkZ < 16; chunkZ++, z++)
        {
            var row = mapImage.GetPixelRowSpan(z);

            for (int chunkX = 0, x = xOffset; chunkX < 16; chunkX++, x++, zxOffset++)
            {
                row[x] = GetBirdseyeBlockColorAt(zxOffset);
            }
        }
    }

    private Rgba32 GetBirdseyeBlockColorAt(int zxOffset)
    {
        const int di = 16 * 16;
        const int maxYInSection = 15 * 16 * 16;

        var yStart = Heightmap[zxOffset];
        var iSectionStart = yStart / 16;
        var iInSection = yStart % 16 * 16 * 16 + zxOffset;

        for (var iSection = iSectionStart; iSection >= 0; iSection--)
        {
            var section = Sections[iSection];
            if (section is null)
            {
                continue;
            }

            var iStart = iSection == iSectionStart
                ? iInSection
                : maxYInSection + zxOffset;

            for (var i = iStart; i >= zxOffset; i -= di)
            {
                var color = section[i];
                if (color.A > 0)
                {
                    return color;
                }
            }
        }

        return Block.BlockColor.Rgba32Transparent;
    }

    private static NbtStream GetValueStream(int compression, byte[] bytes, int index, int count)
    {
        var readStream = new ReadStream();

        var compressedStream = new MemoryStream(bytes, index, count);

        using (var dataStream = GetInflateStream(compression, compressedStream))
            dataStream.CopyTo(readStream);

        return readStream.AsNbtStream();
    }

    private static Stream GetInflateStream(int compression, Stream stream)
        => compression switch
        {
            ZlibCompression => new ZLibStream(stream, CompressionMode.Decompress, true),
            GZipCompression => new GZipStream(stream, CompressionMode.Decompress, true),
            Uncompressed => stream,
            _ => throw new ArgumentOutOfRangeException(nameof(compression))
        };
}
