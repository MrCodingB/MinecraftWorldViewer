using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class LongTagTests
{
    [Fact]
    public void CastsToLongTag()
    {
        new LongTag(5).ToLongTag().Should().BeOfType<LongTag>();
    }

    [Fact]
    public void CastsToLong()
    {
        long l = new LongTag(5);

        l.Should().Be(5);
    }
}
