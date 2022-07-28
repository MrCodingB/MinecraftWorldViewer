using SixLabors.ImageSharp.PixelFormats;

namespace Minecraft.Regions;

public class ChunkSection
{
    private const int TotalBlocks = 16 * 16 * 16;

    private const int MaxBitsPerBlock = 12; // = lb(TotalBlocks)

    private static readonly short[] Masks = new short[MaxBitsPerBlock + 1]
        .Select((_, i) => (short)~(-1 << i))
        .ToArray();

    private static readonly byte[] BlocksPerLong = new byte[MaxBitsPerBlock + 1]
        .Select((_, i) => (byte)(i < 4 ? 0 : 64 / i))
        .ToArray();

    private readonly long[] BlockStates;

    private readonly Block.BlockColor[] BlockPalette;

    private readonly Biome.BiomeInfo[] BiomePalette;

    private readonly long[] BlockIndices;

    private Func<Block.BlockColor[], long[], int, Rgba32> GetBlockFunc;

    public ChunkSection(long[] blockStates, Block.BlockColor[] blockPalette, Biome.BiomeInfo[] biomePalette)
    {
        BlockStates = blockStates;
        BlockPalette = blockPalette;
        BiomePalette = biomePalette;
        BlockIndices = new long[TotalBlocks];
        GetBlockFunc = InitialGetBlockAt;
    }

    /// <summary>
    /// The block colors in this section x, z, y
    /// </summary>
    /// <param name="i">y * 16 * 16 + z * 16 + x</param>
    public Rgba32 this[int i] => GetBlockFunc(BlockPalette, BlockIndices, i);

    private static Rgba32 GetBlockAt(Block.BlockColor[] p, long[] indices, int i) => p[indices[i]].Color;

    private Rgba32 InitialGetBlockAt(Block.BlockColor[] p, long[] b, int i)
    {
        InitializeBlockIndices();

        GetBlockFunc = GetBlockAt;
        return GetBlockAt(BlockPalette, BlockIndices, i);
    }

    private void InitializeBlockIndices()
    {
        var bitsPerBlock = GetBitsPerIndex(BlockPalette.Length);
        var mask = Masks[bitsPerBlock];
        var blocksPerLong = BlocksPerLong[bitsPerBlock];

        var blockStatesIndex = 0;
        var indexInBlockState = 0;
        var l = BlockStates[blockStatesIndex];

        for (var i = 0; i < TotalBlocks; i++)
        {
            if (indexInBlockState >= blocksPerLong)
            {
                l = BlockStates[++blockStatesIndex];

                while (l == 0)
                {
                    i += blocksPerLong;

                    if (i >= TotalBlocks)
                    {
                        return;
                    }

                    l = BlockStates[++blockStatesIndex];
                }

                indexInBlockState = 0;
            }

            BlockIndices[i] = l & mask;

            l >>= bitsPerBlock;
            indexInBlockState++;
        }
    }

    private static int GetBitsPerIndex(int paletteLength)
        => GetBitsPerIndex(paletteLength, 4, 16);

    private static int GetBitsPerIndex(int paletteLength, int minValue, int minPossibleValues)
    {
        var bitsPerBlock = minValue;
        var representableValues = minPossibleValues;

        while (representableValues < paletteLength)
        {
            representableValues *= 2;
            bitsPerBlock++;
        }

        return bitsPerBlock;
    }
}
