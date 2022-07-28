using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class ByteTagTests
{
    [Fact]
    public void CastsToTagTypes()
    {
        var tag = new ByteTag(5);
        tag.ToByteTag().Should().BeOfType<ByteTag>();
        tag.ToShortTag().Should().BeOfType<ShortTag>();
        tag.ToIntTag().Should().BeOfType<IntTag>();
        tag.ToLongTag().Should().BeOfType<LongTag>();
    }

    [Fact]
    public void CastsToNumberTypes()
    {
        var tag = new ByteTag(5);
        byte b = tag;
        short s = tag;
        int i = tag;
        long l = tag;

        b.Should().Be(5);
        s.Should().Be(5);
        i.Should().Be(5);
        l.Should().Be(5);
    }
}
