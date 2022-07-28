using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class EndTagTests
{
    [Fact]
    public void CastsToEndTag()
    {
        new EndTag().ToEndTag().Should().BeOfType<EndTag>();
    }
}
