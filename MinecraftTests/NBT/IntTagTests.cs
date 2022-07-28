using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class IntTagTests
{
    [Fact]
    public void CastsToTagTypes()
    {
        var tag = new IntTag(5);
        tag.ToIntTag().Should().BeOfType<IntTag>();
        tag.ToLongTag().Should().BeOfType<LongTag>();
    }

    [Fact]
    public void CastsToNumberTypes()
    {
        var tag = new IntTag(5);
        int i = tag;
        long l = tag;

        i.Should().Be(5);
        l.Should().Be(5);
    }
}
