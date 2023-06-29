using SixLabors.ImageSharp.PixelFormats;

namespace Minecraft.Regions;

public abstract class ChunkSection
{
    public static ChunkSection FromStatesAndPalette(long[] blockStates, Rgba32[] palette)
        => palette.Length == 1 || blockStates.Length <= 0
            ? new UniformChunkSection(palette[0])
            : new VariedChunkSection(blockStates, palette);

    /// <summary>
    /// The block colors in this section x, z, y
    /// </summary>
    /// <param name="i">y * 16 * 16 + z * 16 + x</param>
    public abstract Rgba32 this[int i] { get; }
}

public class VariedChunkSection : ChunkSection
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

    private readonly Rgba32[] Palette;

    private readonly Rgba32[] BlockColors;

    private Func<Rgba32[], int, Rgba32> GetBlockFunc;

    public VariedChunkSection(long[] blockStates, Rgba32[] palette)
    {
        BlockStates = blockStates;
        Palette = palette;
        BlockColors = new Rgba32[TotalBlocks];
        GetBlockFunc = InitialGetBlockAt;
    }

    public override Rgba32 this[int i] => GetBlockFunc(BlockColors, i);

    private static Rgba32 GetBlockAt(Rgba32[] indices, int i) => indices[i];

    private Rgba32 InitialGetBlockAt(Rgba32[] b, int i)
    {
        InitializeBlockIndices();

        GetBlockFunc = GetBlockAt;
        return GetBlockAt(BlockColors, i);
    }

    private void InitializeBlockIndices()
    {
        var bitsPerBlock = GetBlockSectionBitsPerBlock(Palette.Length);
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

            BlockColors[i] = Palette[l & mask];

            l >>= bitsPerBlock;
            indexInBlockState++;
        }
    }

    private static int GetBlockSectionBitsPerBlock(int paletteLength)
    {
        // Minimal size is 4 bits per block
        var bitsPerBlock = 4;
        var representableValues = 16;

        while (representableValues < paletteLength)
        {
            representableValues *= 2;
            bitsPerBlock++;
        }

        return bitsPerBlock;
    }
}

public class UniformChunkSection : ChunkSection
{
    private readonly Rgba32 BlockColor;

    public UniformChunkSection(Rgba32 blockColor)
    {
        BlockColor = blockColor;
    }

    public override Rgba32 this[int i] => BlockColor;
}
