using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class ShortTagTests
{
    [Fact]
    public void CastsToTagTypes()
    {
        var tag = new ShortTag(5);
        tag.ToShortTag().Should().BeOfType<ShortTag>();
        tag.ToIntTag().Should().BeOfType<IntTag>();
        tag.ToLongTag().Should().BeOfType<LongTag>();
    }

    [Fact]
    public void CastsToNumberTypes()
    {
        var tag = new ShortTag(5);
        short s = tag;
        int i = tag;
        long l = tag;

        s.Should().Be(5);
        i.Should().Be(5);
        l.Should().Be(5);
    }
}
