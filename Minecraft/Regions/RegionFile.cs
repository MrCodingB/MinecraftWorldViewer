using System.Text.RegularExpressions;
using Minecraft.Utils;

namespace Minecraft.Regions;

public class RegionFile : IDisposable
{
    private const int HeaderSize = 8192;

    private static readonly Regex RegionRegex = new(@"r\.(-?\d+)\.(-?\d+)\.mca$");

    private static byte[] RegionFileBuffer = Array.Empty<byte>();

    public int X { get; }

    public int Z { get; }

    public int ChunkCount => ChunkStartOffsets.Count;

    public IList<int> ChunkStartOffsets { get; }

    private FileStream FileStream { get; }

    private RegionFile(int x, int z, IList<int> chunkStartOffsets, FileStream fileStream)
    {
        X = x;
        Z = z;
        ChunkStartOffsets = chunkStartOffsets;
        FileStream = fileStream;
    }

    public static void SetMaxFileSize(long maxFileSize)
    {
        if (maxFileSize > RegionFileBuffer.LongLength)
        {
            RegionFileBuffer = new byte[maxFileSize];
        }
    }

    public static RegionFile? TryLoad(string path)
    {
        var matches = RegionRegex.Match(path);

        if (!matches.Success)
        {
            return null;
        }

        var x = int.Parse(matches.Groups[1].Value);
        var z = int.Parse(matches.Groups[2].Value);

        var regionFileStream = File.OpenRead(path);

        var chunks = ParseRegionFileHeader(regionFileStream);

        return new RegionFile(x, z, chunks, regionFileStream);
    }

    public byte[] ReadFileAndDispose()
    {
        _ = FileStream.Read(RegionFileBuffer, HeaderSize, RegionFileBuffer.Length - HeaderSize);
        FileStream.Dispose();
        return RegionFileBuffer;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FileStream.Dispose();
    }

    private static IList<int> ParseRegionFileHeader(Stream stream)
        => ReadChunkHeaders(ReadHeaderBytes(stream));

    private static byte[] ReadHeaderBytes(Stream stream)
    {
        var bytes = stream.GetBytes(4096);
        stream.Seek(4096, SeekOrigin.Current); // Last 4kiB of header are timestamps => irrelevant
        return bytes;
    }

    private static IList<int> ReadChunkHeaders(byte[] headerBytes)
    {
        var chunkOffsets = new List<int>();

        for (var i = 0; i < 4096; i += 4)
        {
            var offset = GetChunkOffset(headerBytes, i);
            if (offset >= 0)
            {
                chunkOffsets.Add(offset * 4096);
            }
        }

        return chunkOffsets;
    }

    private static int GetChunkOffset(byte[] bytes, int offset)
    {
        var chunkOffset = BitHelper.ToInt24(bytes, offset);
        var length = bytes[offset + 3];

        return length > 0
            ? chunkOffset
            : -1;
    }
}
