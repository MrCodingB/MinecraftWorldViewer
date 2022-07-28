using System;
using System.IO;
using System.IO.Compression;
using Minecraft.NBT;
using Minecraft.Regions;

namespace MinecraftTests.Regions;

public class ChunkTests
{
    [Fact]
    public void ThrowsWhenParsingFromInvalidBufferUncompressed()
    {
        ParsingFromInvalidBufferHelper(s => s, 3);
    }

    [Fact]
    public void ThrowsWhenParsingFromInvalidBufferGzip()
    {
        ParsingFromInvalidBufferHelper(s => new GZipStream(s, CompressionMode.Compress), 1);
    }

    [Fact]
    public void ThrowsWhenParsingFromInvalidBufferZLib()
    {
        ParsingFromInvalidBufferHelper(s => new ZLibStream(s, CompressionMode.Compress), 2);
    }

    [Fact]
    public void ThrowsWhenInvalidCompression()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };

        var func = () => Chunk.FromBytes(10, bytes, 0, bytes.Length);

        func
            .Invoking(f => f())
            .Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    private static void ParsingFromInvalidBufferHelper<T>(Func<Stream, T> compressionStreamFactory, int compression)
        where T : Stream
    {
        var bytes = new byte[]
        {
            (byte)TagType.String,
            0, 3, 46, 48, 47 // Some random name
        };

        var outStream = new MemoryStream();

        using var compressionStream = compressionStreamFactory(outStream);
        compressionStream.Write(bytes);
        compressionStream.Flush();

        outStream.Seek(0, SeekOrigin.Begin);

        var func = () => Chunk.FromBytes(compression, outStream.GetBuffer(), 0, (int)outStream.Length);

        func
            .Invoking(f => f())
            .Should()
            .Throw<InvalidOperationException>();
    }
}
