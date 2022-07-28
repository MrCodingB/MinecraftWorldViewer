using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Minecraft.NBT;
using Minecraft.Utils;

namespace MinecraftTests.NBT;

public class TagSkippingTests
{
    [Fact]
    public void SkipsEndTag()
    {
        SkipsBytesTestHelper(Array.Empty<byte>(), TagType.End);
    }
    
    [Fact]
    public void SkipsByteTag()
    {
        SkipsBytesTestHelper(new byte[] { 1 }, TagType.Byte);
    }

    [Fact]
    public void SkipsShortTag()
    {
        SkipsBytesTestHelper(new byte[] { 1, 2 }, TagType.Short);
    }

    [Fact]
    public void SkipsIntTag()
    {
        SkipsBytesTestHelper(new byte[] { 1, 2, 3, 4 }, TagType.Int);
    }

    [Fact]
    public void SkipsLongTag()
    {
        SkipsBytesTestHelper(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, TagType.Long);
    }

    [Fact]
    public void SkipsFloatTag()
    {
        SkipsBytesTestHelper(new byte[] { 1, 2, 3, 4 }, TagType.Float);
    }

    [Fact]
    public void SkipsDoubleTag()
    {
        SkipsBytesTestHelper(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, TagType.Double);
    }

    [Fact]
    public void SkipsByteArrayTag()
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
        
        SkipsBytesTestHelper(bytes, TagType.ByteArray);
    }

    [Fact]
    public void SkipsStringTag()
    {
        byte[] bytes =
        {
            0, 4,
            61,
            91,
            83,
            17
        };
        
        SkipsBytesTestHelper(bytes, TagType.String);
    }

    [Fact]
    public void SkipsListTag()
    {
        byte[] bytes =
        {
            1,
            0, 0, 0, 6,
            91,
            83,
            17,
            76,
            14,
            2
        };
        
        SkipsBytesTestHelper(bytes, TagType.List);
    }

    [Fact]
    public void SkipsCompoundTag()
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
        
        SkipsBytesTestHelper(bytes, TagType.Compound);
    }

    [Fact]
    public void SkipsIntArrayTag()
    {
        byte[] bytes =
        {
            0, 0, 0, 4,
            0, 0, 0, 91,
            0, 0, 0, 124,
            0, 0, 0, 12,
            0, 0, 0, 87
        };
        
        SkipsBytesTestHelper(bytes, TagType.IntArray);
    }

    [Fact]
    public void SkipsLongArrayTag()
    {
        byte[] bytes =
        {
            0, 0, 0, 2,
            0, 0, 0, 0, 0, 0, 0, 32,
            0, 0, 0, 0, 0, 0, 0, 124
        };
        
        SkipsBytesTestHelper(bytes, TagType.LongArray);
    }

    private static void SkipsBytesTestHelper(IReadOnlyCollection<byte> bytes, TagType tagType)
    {
        var extendedBytes = bytes.Append<byte>(15).ToArray();

        var stream = new NbtStream(extendedBytes);

        stream.SkipTag(tagType);

        stream.Position
            .Should()
            .Be(bytes.Count);
    }
}
