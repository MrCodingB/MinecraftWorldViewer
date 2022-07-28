namespace Minecraft.Regions;

public static class BitConstants
{
    public const int MaxIndices = 16 * 16 * 16;

    public const int MaxBitsPerIndex = 12; // = lb(MaxIndices)

    public static readonly short[] Masks = new short[MaxBitsPerIndex + 1]
        .Select((_, i) => (short)~(-1 << i))
        .ToArray();

    public static readonly byte[] IndicesPerLong = new byte[MaxBitsPerIndex + 1]
        .Select((_, i) => (byte)(i < 4 ? 0 : 64 / i))
        .ToArray();
}

public class SectionPalette<T>
    where T : struct
{
    private readonly int MinIndexLength = 1;

    private readonly int MinRepresentableValues = 2;

    private readonly long[] Data;

    private readonly T[] Palette;

    private readonly long[] Indices;

    private Func<T[], long[], int, T> GetBlockFunc;

    public SectionPalette(long[] data, T[] palette)
    {
        Data = data;
        Palette = palette;
        Indices = new long[BitConstants.MaxIndices];
        GetBlockFunc = InitialGetBlockAt;
    }

    protected SectionPalette(long[] data, T[] palette, int minIndexLenght, int minRepresentableValues)
    {
        Data = data;
        Palette = palette;
        Indices = new long[BitConstants.MaxIndices];
        GetBlockFunc = InitialGetBlockAt;
        MinIndexLength = minIndexLenght;
        MinRepresentableValues = minRepresentableValues;
    }

    public T this[int i] => GetBlockFunc(Palette, Indices, i);

    private static T GetBlockAt(T[] p, long[] indices, int i) => p[indices[i]];

    private T InitialGetBlockAt(T[] p, long[] b, int i)
    {
        InitializeBlockIndices();

        GetBlockFunc = GetBlockAt;
        return GetBlockAt(Palette, Indices, i);
    }

    private void InitializeBlockIndices()
    {
        var bitsPerBlock = GetBitsPerIndex(Palette.Length);
        var mask = BitConstants.Masks[bitsPerBlock];
        var blocksPerLong = BitConstants.IndicesPerLong[bitsPerBlock];

        var blockStatesIndex = 0;
        var indexInBlockState = 0;
        var l = Data[blockStatesIndex];

        for (var i = 0; i < BitConstants.MaxIndices; i++)
        {
            if (indexInBlockState >= blocksPerLong)
            {
                l = Data[++blockStatesIndex];

                while (l == 0)
                {
                    i += blocksPerLong;

                    if (i >= BitConstants.MaxIndices)
                    {
                        return;
                    }

                    l = Data[++blockStatesIndex];
                }

                indexInBlockState = 0;
            }

            Indices[i] = l & mask;

            l >>= bitsPerBlock;
            indexInBlockState++;
        }
    }

    private int GetBitsPerIndex(int paletteLength)
    {
        var bitsPerIndex = MinIndexLength;
        var representableValues = MinRepresentableValues;

        while (representableValues < paletteLength)
        {
            representableValues *= 2;
            bitsPerIndex++;
        }

        return bitsPerIndex;
    }
}

public class SectionBlockPalette : SectionPalette<Block.BlockColor>
{
    public SectionBlockPalette(long[] data, Block.BlockColor[] palette)
         : base(data, palette, 4, 16)
    {
    }
}
