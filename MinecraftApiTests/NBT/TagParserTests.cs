using System;
using System.IO;
using System.Text;
using MinecraftApi.NBT;

namespace MinecraftApiTests.NBT;

public class TagParserTests
{
    [Fact]
    public void ThrowsWhenParsingEndTag()
    {
        byte[] bytes = { 0 };

        new MemoryStream(bytes)
            .Invoking(s => s.GetNamedTag())
            .Should()
            .Throw<ArgumentException>()
            .WithMessage($"Tag of type {TagType.End} cannot be named");
    }

    [Fact]
    public void ParsesByteData()
    {
        byte[] bytes =
        {
            1,
            0, 0,
            67
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.Byte);

        tag.ByteData.Should().Be(67);
    }

    [Fact]
    public void ParsesShortData()
    {
        byte[] bytes =
        {
            2,
            0, 0,
            1, 2
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.Short);

        tag.ShortData.Should().Be(258);
    }

    [Fact]
    public void ParsesIntTag()
    {
        byte[] bytes =
        {
            3,
            0, 0,
            0, 0, 0, 4
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.Int);

        tag.IntData.Should().Be(4);
    }

    [Fact]
    public void ParsesLongTag()
    {
        byte[] bytes =
        {
            4,
            0, 0,
            0, 0, 0, 0, 0, 0, 1, 4
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.Long);

        tag.LongData.Should().Be(260);
    }

    [Fact]
    public void ParsesFloatTag()
    {
        byte[] bytes =
        {
            5,
            0, 0,
            0, 0, 0, 0
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.Float);

        tag.FloatData.Should().Be(0);
    }

    [Fact]
    public void ParsesDoubleTag()
    {
        byte[] bytes =
        {
            6,
            0, 0,
            0, 0, 0, 0, 0, 0, 0, 0
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.Double);

        tag.DoubleData.Should().Be(0);
    }

    [Fact]
    public void ParsesByteArrayTag()
    {
        byte[] bytes =
        {
            7,
            0, 0,
            0, 0, 0, 5,
            14,
            23,
            8,
            165,
            97
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.ByteArray);

        tag.Bytes.Should().BeEquivalentTo(new byte[] { 14, 23, 8, 165, 97 });
    }

    [Fact]
    public void ParsesStringTag()
    {
        byte[] bytes =
        {
            8,
            0, 0,
            0, 4,
            61,
            91,
            83,
            17
        };

        var expectedString = Encoding.UTF8.GetString(bytes[5..]);

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.String);

        tag.StringData.Should().Be(expectedString);
    }

    [Fact]
    public void ParsesListTag()
    {
        byte[] bytes =
        {
            9,
            0, 0,
            1,
            0, 0, 0, 6,
            91,
            83,
            17,
            76,
            14,
            2
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.List);

        tag.ElementType.Should().Be(TagType.Byte);

        tag.Elements.Should().AllBeOfType<DataTag>();
    }

    // [Fact]
    // public void ParsesCompound()
    // {
    //     byte[] bytes = { 0 };
    //
    //     new MemoryStream(bytes)
    //         .Invoking(s => s.GetNamedTag())
    //         .Should()
    //         .Throw<ArgumentException>()
    //         .WithMessage($"Tag of type {TagType.End} cannot be named");
    // }

    [Fact]
    public void ParsesIntArrayTag()
    {
        byte[] bytes =
        {
            11,
            0, 0,
            0, 0, 0, 4,
            0, 0, 0, 91,
            0, 0, 0, 124,
            0, 0, 0, 12,
            0, 0, 0, 87
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.IntArray);

        tag.Integers.Should().BeEquivalentTo(new[] { 91, 124, 12, 87 });
    }

    [Fact]
    public void ParsesLongArrayTag()
    {
        byte[] bytes =
        {
            12,
            0, 0,
            0, 0, 0, 2,
            0, 0, 0, 0, 0, 0, 0, 32,
            0, 0, 0, 0, 0, 0, 0, 124
        };

        var tag = new MemoryStream(bytes).GetNamedTag();

        tag.Type.Should().Be(TagType.LongArray);

        tag.Longs.Should().BeEquivalentTo(new long[] { 32, 124 });
    }
}
