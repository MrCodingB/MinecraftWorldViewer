using System;
using System.IO;
using System.Linq;
using System.Text;
using Minecraft.NBT;
using Minecraft.Utils;

namespace MinecraftTests.NBT;

public class TagParsingTests
{
    [Fact]
    public void ThrowsWhenConvertingToWrongTagType()
    {
        byte[] bytes = { 0 };

        var stream = new NbtStream(bytes);

        var type = stream.GetTagType();

        var tag = stream.GetTag(type);

        tag
            .Invoking(t => t.ToByteArrayTag())
            .Should()
            .Throw<InvalidCastException>();
    }

    [Fact]
    public void ThrowsWhenInvalidTagType()
    {
        var stream = new NbtStream(Array.Empty<byte>());

        stream
            .Invoking(s => s.GetTag((TagType)60))
            .Should()
            .Throw<ArgumentOutOfRangeException>();

        stream
            .Invoking(s => s.SkipTag((TagType)60))
            .Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ParsesByteTag()
    {
        byte[] bytes = { 67 };

        var tag = new NbtStream(bytes).GetTag(TagType.Byte).ToByteTag();

        tag.Type.Should().Be(TagType.Byte);

        tag.Data.Should().Be(67);
    }

    [Fact]
    public void ParsesShortTag()
    {
        byte[] bytes = { 1, 2 };

        var tag = new NbtStream(bytes).GetTag(TagType.Short).ToShortTag();

        tag.Type.Should().Be(TagType.Short);

        tag.Data.Should().Be(258);
    }

    [Fact]
    public void ParsesIntTag()
    {
        byte[] bytes = { 0, 0, 0, 4 };

        var tag = new NbtStream(bytes).GetTag(TagType.Int).ToIntTag();

        tag.Type.Should().Be(TagType.Int);

        tag.Data.Should().Be(4);
    }

    [Fact]
    public void ParsesLongTag()
    {
        byte[] bytes = { 0, 0, 0, 0, 0, 0, 1, 4 };

        var tag = new NbtStream(bytes).GetTag(TagType.Long).ToLongTag();

        tag.Type.Should().Be(TagType.Long);

        tag.Data.Should().Be(260);
    }

    [Fact]
    public void ParsesFloatTag()
    {
        byte[] bytes = { 56, 14, 254, 13 };

        var expectedValue = BitConverter.ToSingle(BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes);

        var tag = new NbtStream(bytes).GetTag(TagType.Float).ToFloatTag();

        tag.Type.Should().Be(TagType.Float);

        tag.Data.Should().Be(expectedValue);
    }

    [Fact]
    public void ParsesDoubleTag()
    {
        byte[] bytes = { 1, 73, 25, 54, 234, 65, 172, 9 };

        var expectedValue = BitConverter.ToDouble(BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes);

        var tag = new NbtStream(bytes).GetTag(TagType.Double).ToDoubleTag();

        tag.Type.Should().Be(TagType.Double);

        tag.Data.Should().Be(expectedValue);
    }

    [Fact]
    public void ParsesByteArrayTag()
    {
        byte[] bytes =
        {
            0, 0, 0, 5,
            14,
            23,
            8,
            165,
            97
        };

        var tag = new NbtStream(bytes).GetTag(TagType.ByteArray).ToByteArrayTag();

        tag.Type.Should().Be(TagType.ByteArray);

        tag.Length.Should().Be(5);

        tag.Should().BeEquivalentTo(new byte[] { 14, 23, 8, 165, 97 });
    }

    [Fact]
    public void ParsesStringTag()
    {
        byte[] bytes =
        {
            0, 4,
            61,
            91,
            83,
            17
        };

        var expectedString = Encoding.UTF8.GetString(bytes[2..]);

        var tag = new NbtStream(bytes).GetTag(TagType.String).ToStringTag();

        tag.Type.Should().Be(TagType.String);

        tag.Data.Should().Be(expectedString);

        tag.Length.Should().Be(expectedString.Length);
    }

    [Fact]
    public void ParsesListTag()
    {
        byte[] bytes =
        {
            1,              // Child type: TagType.Byte = 1
            0, 0, 0, 6,     // Length (Int32) = 6
            91,
            83,
            17,
            76,
            14,
            2
        };

        var tag = new NbtStream(bytes).GetTag(TagType.List).ToListTag();

        tag.Type.Should().Be(TagType.List);

        tag.ItemType.Should().Be(TagType.Byte);

        tag.Count.Should().Be(6);

        tag.Should().AllBeOfType<ByteTag>();
    }

    [Fact]
    public void ParsesEmptyListTag()
    {
        byte[] bytes =
        {
            0,              // Child type: TagType.End = 0
            0, 0, 0, 0      // Length (Int32) = 0
        };

        var tag = new NbtStream(bytes).GetTag(TagType.List).ToListTag();

        tag.Type.Should().Be(TagType.List);
        tag.ItemType.Should().Be(TagType.End);
        tag.Count.Should().Be(0);
    }

    [Fact]
    public void ParsesCompoundTag()
    {
        byte[] bytes =
        {
            1, // 1st child: byte
            0, 4, // name length: 4
            98, // b
            121, // y
            116, // t
            101, // e
            152, // byte value: 152
            0 // end of compound
        };

        var tag = new NbtStream(bytes).GetTag(TagType.Compound).ToCompoundTag();

        tag.Type.Should().Be(TagType.Compound);

        tag.Count.Should().Be(1);

        tag.Should().NotBeEmpty();

        tag.Should().ContainKey("byte");

        tag["byte"].ToByteTag().Data.Should().Be(152);
    }

    [Fact]
    public void ParsesIntArrayTag()
    {
        byte[] bytes =
        {
            0, 0, 0, 4,
            0, 0, 0, 91,
            0, 0, 0, 124,
            0, 0, 0, 12,
            0, 0, 0, 87
        };

        var tag = new NbtStream(bytes).GetTag(TagType.IntArray).ToIntArrayTag();

        tag.Type.Should().Be(TagType.IntArray);

        tag.Length.Should().Be(4);

        tag.Should().BeEquivalentTo(new[] { 91, 124, 12, 87 });
    }

    [Fact]
    public void ParsesLongArrayTag()
    {
        byte[] bytes =
        {
            0, 0, 0, 2,
            0, 0, 0, 0, 0, 0, 0, 32,
            0, 0, 0, 0, 0, 0, 0, 124
        };

        var tag = new NbtStream(bytes).GetTag(TagType.LongArray).ToLongArrayTag();

        tag.Type.Should().Be(TagType.LongArray);

        tag.Length.Should().Be(2);

        tag.Should().BeEquivalentTo(new long[] { 32, 124 });
    }
}
