using System;
using System.IO;
using System.Linq;
using System.Text;
using Minecraft.NBT;
using Minecraft.Utils;

namespace MinecraftTests;

public class NbtStreamTests
{
    [Fact]
    public void ConstructsWithCorrectValues()
    {
        var stream = new NbtStream(new byte[] { 15, 23, 61 });

        stream.CanSeek.Should().Be(true);
        stream.CanWrite.Should().Be(false);
        stream.CanRead.Should().Be(true);
        stream.Length.Should().Be(3);
        stream.Position.Should().Be(0);

        stream
            .Invoking(s => s.Position = 1)
            .Should()
            .Throw<NotSupportedException>();
    }

    [Fact]
    public void SeeksToCorrectPositions()
    {
        var stream = new NbtStream(new byte[] { 15, 23, 61 });

        stream.Seek(2, SeekOrigin.Begin);
        stream.Position.Should().Be(2);

        stream
            .Invoking(s => s.Seek(-1, SeekOrigin.Current))
            .Should()
            .Throw<NotSupportedException>();

        stream.Seek(0, SeekOrigin.End);
        stream.Position.Should().Be(3);
    }

    [Fact]
    public void ThrowsWhenCallingUnsupportedMethods()
    {
        var stream = new NbtStream(Array.Empty<byte>());

        stream
            .Invoking(s => s.Flush())
            .Should()
            .Throw<NotSupportedException>();

        stream
            .Invoking(s => s.SetLength(19))
            .Should()
            .Throw<NotSupportedException>();

        stream
            .Invoking(s => s.Write(new byte[] { 4, 13 }))
            .Should()
            .Throw<NotSupportedException>();
    }

    [Fact]
    public void GetsByteAndThrowsWhenEmpty()
    {
        var stream = new NbtStream(new byte[] { 14, 145 });

        stream.GetByte().Should().Be(14);
        stream.GetByte().Should().Be(145);

        stream
            .Invoking(s => s.GetByte())
            .Should()
            .Throw<EndOfStreamException>();
    }

    [Fact]
    public void GetBytesAndThrowsWhenEmpty()
    {
        var stream = new NbtStream(new byte[] { 14, 145, 65, 10, 205, 143 });

        stream.GetBytes(2).Should().BeEquivalentTo(new byte[] { 14, 145 });
        stream.GetBytes(4).Should().BeEquivalentTo(new byte[] { 65, 10, 205, 143 });

        stream
            .Invoking(s => s.GetBytes(5))
            .Should()
            .Throw<EndOfStreamException>();
    }

    [Fact]
    public void GetsMoreThan8BytesUsingBufferBlockCopy()
    {
        var bytes = new byte[] { 14, 145, 65, 10, 205, 143, 124, 43, 68, 193, 128 };

        new NbtStream(bytes)
            .GetBytes(bytes.Length)
            .Should()
            .BeEquivalentTo(bytes);
    }

    [Fact]
    public void ReadsByte()
    {
        new NbtStream(new byte[] { 0x32 })
            .ReadByte()
            .Should()
            .Be(0x32);
    }

    [Fact]
    public void GetsUInt16()
    {
        new NbtStream(new byte[] { 0x32, 0xD3 })
            .GetUInt16()
            .Should()
            .Be(0x32D3);
    }

    [Fact]
    public void GetsInt16()
    {
        new NbtStream(new byte[] { 0xFF, 0xFE })
            .GetInt16()
            .Should()
            .Be(-2);
    }

    [Fact]
    public void GetsInt32()
    {
        new NbtStream(new byte[] { 0x3B, 0x32, 0x21, 0x1E })
            .GetInt32()
            .Should()
            .Be(0x3B32211E);
    }

    [Fact]
    public void GetsInt64()
    {
        new NbtStream(new byte[] { 0x1F, 0xA5, 0x54, 0, 0, 0, 0x1, 0x10 })
            .GetInt64()
            .Should()
            .Be(0x1FA5540000000110);
    }

    [Fact]
    public void GetsFloat()
    {
        const float value = 15153249.124547248F;

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        new NbtStream(bytes)
            .GetFloat()
            .Should()
            .Be(value);
    }

    [Fact]
    public void GetsDouble()
    {
        const double value = -165465165.8512318359;

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        new NbtStream(bytes)
            .GetDouble()
            .Should()
            .Be(value);
    }

    [Fact]
    public void GetsString()
    {
        const string value = "Stuff 30 a j48t 209";

        var bytes = new byte[] { 0, (byte)value.Length }
            .Concat(Encoding.UTF8.GetBytes(value))
            .ToArray();

        new NbtStream(bytes)
            .GetString()
            .Should()
            .Be(value);
    }

    [Fact]
    public void SkipsNBytes()
    {
        var bytes = new byte[]
        {
            1, 2, 3, 4
        };

        var expandedBytes = bytes.Append<byte>(0).ToArray();

        var stream = new NbtStream(expandedBytes);

        stream.Skip(bytes.Length);

        stream.Position
            .Should()
            .Be(bytes.Length);
    }
}
