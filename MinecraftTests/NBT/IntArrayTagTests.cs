using System;
using System.Collections;
using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class IntArrayTagTests
{
    [Fact]
    public void ShouldHaveCorrectLength()
    {
        new IntArrayTag(new[] { 1, 2 }).Length
            .Should()
            .Be(2);
    }

    [Fact]
    public void CanEnumerate()
    {
        var tag = new IntArrayTag(new[] { 1, 2 });

        var i = 0;

        foreach (var _ in tag)
        {
            i++;
        }

        i.Should().Be(tag.Length);
    }

    [Fact]
    public void CastsToIntArrayTag()
    {
        new IntArrayTag(Array.Empty<int>())
            .ToIntArrayTag()
            .Should()
            .BeOfType<IntArrayTag>();
    }

    [Fact]
    public void CastsToIntArray()
    {
        int[] ints = new IntArrayTag(Array.Empty<int>());
        
        ints
            .Should()
            .NotBeNull();
    }
}
