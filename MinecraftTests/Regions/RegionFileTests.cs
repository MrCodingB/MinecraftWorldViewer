using System;
using System.IO;
using Minecraft.Regions;

namespace MinecraftTests.Regions;

public class RegionFileTests
{
    [Fact]
    public void ReadsRegionFileAndParsesChunkRootTag()
    {
        var regionFile = RegionFile.Load(Path.Combine(Environment.CurrentDirectory, "r.0.0.mca"));

        regionFile.Should().NotBeNull();

        regionFile.X.Should().Be(0);
        regionFile.Z.Should().Be(0);

        regionFile.ChunkCount.Should().NotBe(0);
    }
}
