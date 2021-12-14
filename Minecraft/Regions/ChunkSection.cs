using Minecraft.NBT;

namespace Minecraft.Regions;

public class ChunkSection
{
    public int Y { get; }

    private string[] Palette { get; }

    private long[] BlockStates { get; }

    private string?[,] BirdseyeBlocks { get; }

    private ChunkSection(int yIndex, long[] blockStates, string[] palette)
    {
        Y = yIndex;
        BlockStates = blockStates;
        Palette = palette;
        BirdseyeBlocks = new string[16, 16];

        if (BlockStates.Length == 0)
        {
            for (var x = 0; x < 16; x++)
            for (var z = 0; z < 16; z++)
                BirdseyeBlocks[x, z] = Palette[0];
        }
        else
        {
            SetBirdseyeView();
        }
    }

    public static ChunkSection? FromTag(CompoundTag tag)
    {
        var y = (sbyte)tag["Y"].ToByteTag();

        if (!tag.ContainsKey("block_states"))
        {
            return null;
        }

        var blockStates = tag["block_states"].ToCompoundTag();

        var palette = GetPalette(blockStates);

        if (palette.Length <= 1 && palette.All(b => b == Blocks.Air))
        {
            return null;
        }

        long[] blockStatesValue = blockStates.ContainsKey("data")
            ? blockStates["data"].ToLongArrayTag()
            : Array.Empty<long>();

        return new ChunkSection(y, blockStatesValue, palette);
    }

    public string? this[int x, int z] => BirdseyeBlocks[x, z];

    private static string[] GetPalette(CompoundTag blockStates)
    {
        var palette = blockStates["palette"].ToListTag();

        var result = new string[palette.Count];

        for (var i = 0; i < palette.Count; i++)
        {
            var block = palette[i].ToCompoundTag();

            result[i] = block["Name"].ToStringTag();
        }

        return result;
    }

    private void SetBirdseyeView()
    {
        var bitsPerBlock = GetBlockSectionBitsPerBlock();
        var mask = ~(-1 << bitsPerBlock);
        var blocksPerLong = 64 / bitsPerBlock;
        var maxIndexInBlock = blocksPerLong - 1;
        var totalBlockCapacity = blocksPerLong * BlockStates.Length;
        var spareBlocks = totalBlockCapacity - 4096;
        var startingIndex = maxIndexInBlock - spareBlocks;

        var blockIndex = BlockStates.Length - 1;
        var indexInBlock = startingIndex;

        for (var y = 15; y >= 0; y--)
        for (var z = 15; z >= 0; z--)
        for (var x = 15; x >= 0; x--)
        {
            if (indexInBlock < 0)
            {
                blockIndex--;
                indexInBlock = maxIndexInBlock;
            }
            
            if (BirdseyeBlocks[x, z] is null)
            {
                var l = BlockStates[blockIndex];
                var index = (l >> (indexInBlock * bitsPerBlock)) & mask;
                var blockName = Palette[index];

                if (Block.IsVisibleOnMap(blockName))
                {
                    BirdseyeBlocks[x, z] = blockName;
                }
            }

            indexInBlock--;
        }
    }
    
    private int GetBlockSectionBitsPerBlock()
    {
        // Minimal size is 4 bits per block
        var bitsPerBlock = 4;
        var maxRepresentableValues = 16;

        while (maxRepresentableValues < Palette.Length)
        {
            maxRepresentableValues *= 2;
            bitsPerBlock++;
        }

        return bitsPerBlock;
    }
}
