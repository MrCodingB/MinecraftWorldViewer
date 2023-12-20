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

public sealed class VariedChunkSection : ChunkSection
{
    private const int MaxBitsPerBlock = 12; // = lb(TotalBlocks) = lb(16 * 16 * 16)

    private static readonly short[] Masks = new short[MaxBitsPerBlock + 1]
        .Select((_, i) => (short)~(-1 << i))
        .ToArray();

    private static readonly byte[] BlocksPerLongValues = new byte[MaxBitsPerBlock + 1]
        .Select((_, i) => (byte)(i < 4 ? 0 : 64 / i))
        .ToArray();

    private readonly long[] BlockStates;

    private readonly Rgba32[] Palette;

    private readonly int BitsPerBlock;

    private readonly short BlockStateMask;

    private readonly byte BlocksPerLong;

    public VariedChunkSection(long[] blockStates, Rgba32[] palette)
    {
        BlockStates = blockStates;
        Palette = palette;

        BitsPerBlock = GetBlockSectionBitsPerBlock(Palette.Length);
        BlockStateMask = Masks[BitsPerBlock];
        BlocksPerLong = BlocksPerLongValues[BitsPerBlock];
    }

    public override Rgba32 this[int i] => GetBlockColorAt(i);

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

    private Rgba32 GetBlockColorAt(int i)
    {
        var longIndex = i / BlocksPerLong;
        var indexInBlockSate = i % BlocksPerLong;

        var l = BlockStates[longIndex];
        var paletteIndex = (l >> (indexInBlockSate * BitsPerBlock)) & BlockStateMask;

        return Palette[paletteIndex];
    }
}

public sealed class UniformChunkSection : ChunkSection
{
    private readonly Rgba32 BlockColor;

    public UniformChunkSection(Rgba32 blockColor)
    {
        BlockColor = blockColor;
    }

    public override Rgba32 this[int i] => BlockColor;
}
