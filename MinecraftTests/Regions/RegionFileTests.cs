using System;
using System.IO;
using Minecraft.Regions;

namespace MinecraftTests.Regions;

public class RegionFileTests
{
    private static readonly string TestRegionPath =
        Path.Combine(Environment.CurrentDirectory, "TestChunk-Region", "r.-1.-1.mca");

    [Fact]
    public void ReadsRegionFileAndParsesChunkRootTag()
    {
        var regionFile = RegionFile.TryLoad(TestRegionPath);

        regionFile.Should().NotBeNull();

        regionFile!.X.Should().Be(-1);
        regionFile.Z.Should().Be(-1);

        regionFile.ChunkCount.Should().NotBe(0);
    }

    [Fact]
    public void LoadingInvalidPathReturnsNull()
    {
        RegionFile
            .TryLoad("some/invalid/path")
            .Should()
            .BeNull();
    }
}
