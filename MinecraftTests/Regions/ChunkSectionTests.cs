using System;
using Minecraft.Regions;
using SixLabors.ImageSharp.PixelFormats;

namespace MinecraftTests.Regions;

public class ChunkSectionTests
{
    [Fact]
    public void CreatesVariedChunkSection()
    {
        ChunkSection
            .FromStatesAndPalette(new[] { 1L, 2L }, new Rgba32[] { new(1, 2, 3), new(4, 5, 6) })
            .Should()
            .BeOfType<VariedChunkSection>();
    }

    [Fact]
    public void CreatesUniformChunkSection()
    {
        ChunkSection
            .FromStatesAndPalette(Array.Empty<long>(), new Rgba32[] { new(1, 2, 3) })
            .Should()
            .BeOfType<UniformChunkSection>();
    }

    [Fact]
    public void GetsBlocksVaried()
    {
        // 4 Bits per block, 16 * 16 * 16 = 4096 Blocks
        var blockStates = new long[256];
        var section = ChunkSection.FromStatesAndPalette(blockStates, new Rgba32[] { new(1, 2, 3), new(4, 5, 6) });

        for (var x = 0; x < 16; x++)
        for (var y = 0; y < 16; y++)
        for (var z = 0; z < 16; z++)
            section[y * 16 * 16 + z * 16 + x].Should().Be(new Rgba32(1, 2, 3));
    }

    [Fact]
    public void GetsBlocksUniform()
    {
        var section = ChunkSection.FromStatesAndPalette(Array.Empty<long>(), new Rgba32[] { new(1, 2, 3) });

        for (var x = 0; x < 16; x++)
        for (var y = 0; y < 16; y++)
        for (var z = 0; z < 16; z++)
            section[y * 16 * 16 + z * 16 + x].Should().Be(new Rgba32(1, 2, 3));
    }
}
