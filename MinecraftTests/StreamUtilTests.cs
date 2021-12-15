using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MinecraftTests;

public class StreamUtilTests
{
    [Fact]
    public void GetsByteAndThrowsWhenEmpty()
    {
        var stream = new MemoryStream(new byte[] { 14, 145, 65 });

        stream.GetByte().Should().Be(14);
        stream.GetByte().Should().Be(145);
        stream.GetByte().Should().Be(65);

        stream
            .Invoking(s => s.GetByte())
            .Should()
            .Throw<EndOfStreamException>();
    }

    [Fact]
    public void GetBytesAndThrowsWhenEmpty()
    {
        var stream = new MemoryStream(new byte[] { 14, 145, 65, 10, 205, 143 });

        stream.GetBytes(3).Should().BeEquivalentTo(new byte[] { 14, 145, 65 });
        stream.GetBytes(2).Should().BeEquivalentTo(new byte[] { 10, 205 });

        stream
            .Invoking(s => s.GetBytes(5))
            .Should()
            .Throw<EndOfStreamException>();
    }

    [Fact]
    public void GetsUInt16()
    {
        new MemoryStream(new byte[] { 0, 145 })
            .GetUInt16()
            .Should()
            .Be(145);
    }

    [Fact]
    public void GetsInt16()
    {
        new MemoryStream(new byte[] { 255, 254 })
            .GetInt16()
            .Should()
            .Be(-2);
    }

    [Fact]
    public void GetsInt32()
    {
        new MemoryStream(new byte[] { 0, 0, 2, 0 })
            .GetInt32()
            .Should()
            .Be(512);
    }

    [Fact]
    public void GetsInt64()
    {
        new MemoryStream(new byte[] { 0, 0, 0, 0, 0, 0, 1, 10 })
            .GetInt64()
            .Should()
            .Be(266);
    }

    [Fact]
    public void GetsFloat()
    {
        const float value = 149.18F;

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        
        new MemoryStream(bytes)
            .GetFloat()
            .Should()
            .Be(value);
    }

    [Fact]
    public void GetsDouble()
    {
        const double value = -1.8359;

        var bytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        
        new MemoryStream(bytes)
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

        new MemoryStream(bytes)
            .GetString()
            .Should()
            .Be(value);
    }
}
