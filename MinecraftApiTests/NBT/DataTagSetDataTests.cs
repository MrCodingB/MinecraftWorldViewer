using System;
using System.Collections.Generic;
using MinecraftApi.NBT;

namespace MinecraftApiTests.NBT;

public class DataTagSetDataTests
{
    [Fact]
    public void SetsByteData()
    {
        const byte value = 142;

        var tag = new DataTag(TagType.Byte);

        tag.SetData(value);

        tag.ByteData.Should().Be(value);
    }

    [Fact]
    public void SetsShortData()
    {
        const short value = 22648;

        var tag = new DataTag(TagType.Short);

        tag.SetData(value);

        tag.ShortData.Should().Be(value);
    }

    [Fact]
    public void SetsIntData()
    {
        const int value = 14239842;

        var tag = new DataTag(TagType.Int);

        tag.SetData(value);

        tag.IntData.Should().Be(value);
    }

    [Fact]
    public void SetsLongData()
    {
        const long value = 145444132135471162;

        var tag = new DataTag(TagType.Long);

        tag.SetData(value);

        tag.LongData.Should().Be(value);
    }

    [Fact]
    public void SetsFloatData()
    {
        const float value = 142.19283f;

        var tag = new DataTag(TagType.Float);

        tag.SetData(value);

        tag.FloatData.Should().Be(value);
    }

    [Fact]
    public void SetsDoubleData()
    {
        const double value = 142.0293;

        var tag = new DataTag(TagType.Double);

        tag.SetData(value);

        tag.DoubleData.Should().Be(value);
    }

    [Fact]
    public void SetsByteArrayData()
    {
        byte[] value = { 142, 19, 248, 53 };

        var tag = new DataTag(TagType.ByteArray);

        tag.SetData(value);

        tag.Bytes.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void SetsStringData()
    {
        const string value = "Lorem ipsum dolor sit amet";

        var tag = new DataTag(TagType.String);

        tag.SetData(value);

        tag.StringData.Should().Be(value);
    }

    [Fact]
    public void SetsListData()
    {
        DataTag[] value =
        {
            new(TagType.Double),
            new(TagType.Double)
        };

        var tag = new DataTag(TagType.List);

        tag.SetData(value);

        tag.Elements.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void ThrowsWhenSettingDifferentListElementTypes()
    {
        DataTag[] value =
        {
            new(TagType.Double),
            new(TagType.Long)
        };

        var tag = new DataTag(TagType.List);

        tag.Invoking(t => t.SetData(value))
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void SetsCompoundData()
    {
        var value = new Dictionary<string, NamedTag>
        {
            { "stuff", new NamedTag(TagType.Byte, "stuff") },
            { "other", new NamedTag(TagType.Long, "other") }
        };

        var tag = new DataTag(TagType.Compound);

        tag.SetData(value);

        tag.Children.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void SetsIntArrayData()
    {
        int[] value = { 13, 268, 3691, 17824 };

        var tag = new DataTag(TagType.IntArray);

        tag.SetData(value);

        tag.Integers.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void SetsLongArrayData()
    {
        long[] value = { 13, 24, 5834, 159 };

        var tag = new DataTag(TagType.LongArray);

        tag.SetData(value);

        tag.Longs.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void ThrowsWhenSettingInvalidData()
    {
        const int value = 182;

        var tag = new DataTag(TagType.List);

        tag.Invoking(t => t.SetData(value))
            .Should()
            .Throw<InvalidOperationException>();
    }
}
