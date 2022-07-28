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

    private static readonly ChunkParser Parser = new();

    private static readonly ReadStream ReadStream = new();

    private int X { get; }

    private int Z { get; }

    private IList<ChunkSection> Sections { get; }

    public Chunk(int x, int z, IList<ChunkSection> sections)
    {
        X = x;
        Z = z;
        Sections = sections;
    }

    public static Chunk FromBytes(int compression, byte[] bytes, int index, int count)
    {
        using var stream = GetValueStream(compression, bytes, index, count);

        var type = stream.GetTagType();
        var nameLength = stream.GetUInt16();

        if (type != TagType.Compound || nameLength != 0)
        {
            throw new InvalidOperationException("Invalid root tag for chunk");
        }

        return Parser.ParseChunk(stream);
    }

    public void DrawChunk(Image<Rgba32> mapImage, int x0, int z0)
    {
        var xOffset = X * 16 - x0;
        var zOffset = Z * 16 - z0;

        var xzOffset = 0;

        for (int chunkZ = 0, z = zOffset; chunkZ < 16; chunkZ++, z++)
        {
            var row = mapImage.GetPixelRowSpan(z);

            for (int chunkX = 0, x = xOffset; chunkX < 16; chunkX++, x++, xzOffset++)
            {
                row[x] = GetBirdseyeBlockColorAt(xzOffset);
            }
        }
    }

    private Rgba32 GetBirdseyeBlockColorAt(int xzOffset)
    {
        const int di = 16 * 16;
        const int iYMax = 15 * 16 * 16; // i at max y (y = 15)

        var iStart = iYMax + xzOffset;

        for (var iSection = Sections.Count - 1; iSection >= 0; iSection--)
        {
            var section = Sections[iSection];

            for (var i = iStart; i >= xzOffset; i -= di)
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
        ReadStream.SetLength(0);

        var compressedStream = new MemoryStream(bytes, index, count);

        using (var dataStream = GetInflateStream(compression, compressedStream))
            dataStream.CopyTo(ReadStream);

        return ReadStream.AsNbtStream();
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
