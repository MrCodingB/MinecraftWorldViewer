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

        var result = mapGenerator.Generate();

        result.Should().NotBeNull();
        
        result.Mutate(r =>
        {
            r.Crop(new Rectangle(result.Width - 16, result.Height - 16, 16, 16));
        });

        var blackConcrete = Block.BlockColors[Blocks.BlackConcrete].MapColor!.Value;
        var orangeWool = Block.BlockColors[Blocks.OrangeWool].MapColor!.Value;
        var redWool = Block.BlockColors[Blocks.RedWool].MapColor!.Value;
        var lightBlueWool = Block.BlockColors[Blocks.LightBlueWool].MapColor!.Value;
        var blueWool = Block.BlockColors[Blocks.BlueWool].MapColor!.Value;
        var pinkWool = Block.BlockColors[Blocks.PinkWool].MapColor!.Value;
        var magentaWool = Block.BlockColors[Blocks.MagentaWool].MapColor!.Value;
        var limeWool = Block.BlockColors[Blocks.LimeWool].MapColor!.Value;
        var greenWool = Block.BlockColors[Blocks.GreenWool].MapColor!.Value;

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
