using System;
using System.Collections;
using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class LongArrayTagTests
{
    [Fact]
    public void ShouldHaveCorrectLength()
    {
        new LongArrayTag(new long[] { 1, 2 }).Length
            .Should()
            .Be(2);
    }

    [Fact]
    public void CanEnumerate()
    {
        var tag = new LongArrayTag(new long[] { 1, 2 });

        var i = 0;

        foreach (var _ in tag)
        {
            i++;
        }

        i.Should().Be(tag.Length);
        tag.Should().BeEquivalentTo(new long[] { 1, 2 });
    }

    [Fact]
    public void CastsToLongArrayTag()
    {
        new LongArrayTag(Array.Empty<long>())
            .ToLongArrayTag()
            .Should()
            .BeOfType<LongArrayTag>();
    }

    [Fact]
    public void CastsToIntArray()
    {
        long[] longs = new LongArrayTag(Array.Empty<long>());
        
        longs
            .Should()
            .NotBeNull();
    }
}
