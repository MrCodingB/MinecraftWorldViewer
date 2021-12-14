using System.IO.Compression;
using Minecraft.NBT;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Minecraft.Regions;

public class Chunk
{
    private const byte GZipCompression = 1;
    private const byte ZlibCompression = 2;
    private const byte Uncompressed = 3;

    public int X { get; }

    public int Z { get; }

    private IList<ChunkSection> Sections { get; }

    private Chunk(int x, int z, IList<ChunkSection> sections)
    {
        X = x;
        Z = z;
        Sections = sections;
    }

    public static Chunk FromStream(Stream stream)
    {
        var tree = TagTree.FromStream(GetDataStream(stream));

        var root = tree.Root;

        if (root.ContainsKey("Level"))
        {
            root = root["Level"].ToCompoundTag();
        }

        var x = root["xPos"].ToIntTag();
        var z = root["zPos"].ToIntTag();

        return new Chunk(x, z, GetChunkSections(root));
    }

    public Image<Rgba32> CalculateBitmap()
    {
        var bitmap = new Image<Rgba32>(16, 16);

        for (var z = 0; z < 16; z++)
        {
            var row = bitmap.GetPixelRowSpan(z);

            for (var x = 0; x < 16; x++)
            {
                for (var i = Sections.Count - 1; i >= 0; i--)
                {
                    var section = Sections[i];

                    var block = section[x, z];
                    var color = block is not null ? Block.BlockColors[block].MapColor : null;
                    if (color is not null)
                    {
                        row[x] = color.Value;
                        break;
                    }
                }
            }
        }

        ProgressManager.CompletedChunk();

        return bitmap;
    }

    private static IList<ChunkSection> GetChunkSections(CompoundTag root)
    {
        if (!root.ContainsKey("sections"))
        {
            return new List<ChunkSection>();
        }

        return root["sections"]
            .ToListTag()
            .Select(item => ChunkSection.FromTag(item.ToCompoundTag()))
            .Where(chunkSection => chunkSection is not null)
            .OrderBy(s => s!.Y)
            .ToList()!;
    }

    private static Stream GetDataStream(Stream chunkStream)
    {
        var compression = chunkStream.GetByte();

        using var dataStream = compression switch
        {
            GZipCompression => new GZipStream(chunkStream, CompressionMode.Decompress),
            ZlibCompression => new ZLibStream(chunkStream, CompressionMode.Decompress),
            _ => chunkStream
        };

        var memoryStream = new MemoryStream();

        dataStream.CopyTo(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}
