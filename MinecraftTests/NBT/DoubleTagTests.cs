using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class DoubleTagTests
{
    [Fact]
    public void CastsToDoubleTag()
    {
        new DoubleTag(5.38).ToDoubleTag().Should().BeOfType<DoubleTag>();
    }

    [Fact]
    public void CastsToDouble()
    {
        double d = new DoubleTag(5.38);

        d.Should().Be(5.38);
    }
}
