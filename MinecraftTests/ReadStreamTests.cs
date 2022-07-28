using System;
using System.IO;
using Minecraft.Utils;

namespace MinecraftTests;

public class ReadStreamTests
{
    [Fact]
    public void ConstructsWithCorrectValues()
    {
        var stream = new ReadStream();

        stream.CanRead.Should().Be(false);
        stream.CanSeek.Should().Be(false);
        stream.CanWrite.Should().Be(true);
        stream.Length.Should().Be(0);
        stream.Position.Should().Be(0);

        stream
            .Invoking(s => s.Position = 1)
            .Should()
            .Throw<NotSupportedException>();
    }
    
    [Fact]
    public void ThrowsWhenCallingNotSupportedMethods()
    {
        var stream = new ReadStream();

        stream
            .Invoking(s => s.Read(Array.Empty<byte>(), 0, 0))
            .Should()
            .Throw<NotSupportedException>();

        stream
            .Invoking(s => s.CopyTo(new MemoryStream()))
            .Should()
            .Throw<NotSupportedException>();

        stream
            .Invoking(s => s.Seek(0, SeekOrigin.Begin))
            .Should()
            .Throw<NotSupportedException>();
    }
    
    [Fact]
    public void ThrowsWhenWritingToDisposedStream()
    {
        var stream = new ReadStream();

        stream.Flush();
        stream.Dispose();
        
        stream
            .Invoking(s => s.Write(Array.Empty<byte>(), 0, 0))
            .Should()
            .Throw<ObjectDisposedException>();
    }
    
    [Fact]
    public void IncreasesBuffersWhenWriting()
    {
        var stream = new ReadStream();
        
        var bytes = new byte[ReadStream.DefaultInitialCapacity];

        stream.Write(bytes, 0, bytes.Length);

        stream.Length.Should().Be(bytes.Length);
        stream.Position.Should().Be(bytes.Length);

        // This causes a resize with Buffer.BlockCopy
        stream.Write(bytes, 0, bytes.Length);

        stream.Length.Should().Be(bytes.Length * 2);
        stream.Position.Should().Be(bytes.Length * 2);
    }
}
