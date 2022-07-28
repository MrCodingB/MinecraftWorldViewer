using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MinecraftTests;

public class MapGeneratorTests
{
    [Fact]
    public void GeneratesTestChunk()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "TestChunk-Region");

        var mapGenerator = new MapGenerator(path);

        var progressEvents = 0;

        ProgressManager.Progress += (_, _) => progressEvents++;

        var result = mapGenerator.Generate();

        progressEvents.Should().BeGreaterThan(0);

        ProgressManager.AvgMemoryInMb.Should().BeGreaterThan(0);

        ProgressManager.TimePerChunk.Should().BeGreaterThan(TimeSpan.Zero);
        ProgressManager.TimePerRegion.Should().BeGreaterThan(TimeSpan.Zero);
        ProgressManager.Reset();
        ProgressManager.TimePerChunk.Should().Be(TimeSpan.Zero);
        ProgressManager.TimePerRegion.Should().Be(TimeSpan.Zero);

        result.Should().NotBeNull();

        result.Mutate(r => r.Crop(new Rectangle(result.Width - 16, result.Height - 16, 16, 16)));

        var blackConcrete = Block.BlockColors[Blocks.BlackConcrete].Color;
        var orangeWool = Block.BlockColors[Blocks.OrangeWool].Color;
        var redWool = Block.BlockColors[Blocks.RedWool].Color;
        var lightBlueWool = Block.BlockColors[Blocks.LightBlueWool].Color;
        var blueWool = Block.BlockColors[Blocks.BlueWool].Color;
        var pinkWool = Block.BlockColors[Blocks.PinkWool].Color;
        var magentaWool = Block.BlockColors[Blocks.MagentaWool].Color;
        var limeWool = Block.BlockColors[Blocks.LimeWool].Color;
        var greenWool = Block.BlockColors[Blocks.GreenWool].Color;

        var expectedColors = new Color[16, 16];

        for (var x = 0; x < 16; x++)
        for (var z = 0; z < 16; z++)
            expectedColors[x, z] = blackConcrete;

        expectedColors[0, 0] = orangeWool;
        expectedColors[3, 3] = redWool;
        expectedColors[6, 6] = orangeWool;
        expectedColors[9, 6] = lightBlueWool;
        expectedColors[12, 3] = blueWool;
        expectedColors[15, 0] = lightBlueWool;
        expectedColors[6, 9] = pinkWool;
        expectedColors[3, 12] = magentaWool;
        expectedColors[0, 15] = pinkWool;
        expectedColors[9, 9] = limeWool;
        expectedColors[12, 12] = greenWool;
        expectedColors[15, 15] = limeWool;

        for (var x = 0; x < 16; x++)
        for (var z = 0; z < 16; z++)
            result[x, z].Should().Be(expectedColors[x, z].ToPixel<Rgba32>(), $"images should match at ({x}|{z})");
    }
}
