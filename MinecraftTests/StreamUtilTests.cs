using System.IO;
using Minecraft.Utils;

namespace MinecraftTests;

public class StreamUtilTests
{
    [Fact]
    public void GetBytesAndThrowsWhenEmpty()
    {
        var stream = new MemoryStream(new byte[] { 14, 145, 65, 10, 205, 143 });

        stream.GetBytes(3).Should().BeEquivalentTo(new byte[] { 14, 145, 65 });
        stream.GetBytes(2).Should().BeEquivalentTo(new byte[] { 10, 205 });

        stream
            .Invoking(s => s.GetBytes(5))
            .Should()
            .Throw<EndOfStreamException>();
    }
}
