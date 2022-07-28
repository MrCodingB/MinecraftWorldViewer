using System;
using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class ByteArrayTagTests
{
    [Fact]
    public void ShouldHaveCorrectLength()
    {
        new ByteArrayTag(new byte[] { 1, 2 }).Length
            .Should()
            .Be(2);
    }

    [Fact]
    public void CanEnumerate()
    {
        var tag = new ByteArrayTag(new byte[] { 1, 2 });

        var i = 0;

        foreach (var _ in tag)
        {
            i++;
        }

        i.Should().Be(tag.Length);
    }

    [Fact]
    public void CastsToByteArrayTag()
    {
        new ByteArrayTag(Array.Empty<byte>())
            .ToByteArrayTag()
            .Should()
            .BeOfType<ByteArrayTag>();
    }

    [Fact]
    public void CastsToByteArray()
    {
        byte[] bytes = new ByteArrayTag(Array.Empty<byte>());
        
        bytes
            .Should()
            .NotBeNull();
    }
}
