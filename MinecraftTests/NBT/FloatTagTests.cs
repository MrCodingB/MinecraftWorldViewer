using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class FloatTagTests
{
    [Fact]
    public void CastsToTagTypes()
    {
        var tag = new FloatTag(5.38f);
        
        tag.ToFloatTag().Should().BeOfType<FloatTag>();
        tag.ToDoubleTag().Should().BeOfType<DoubleTag>();
    }

    [Fact]
    public void CastsToNumberTypes()
    {
        var tag = new FloatTag(5.38f);
        float f = tag;
        double d = tag;

        f.Should().Be(5.38f);
        d.Should().Be(5.38f);
    }
}
