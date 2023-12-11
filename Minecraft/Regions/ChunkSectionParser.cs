using Minecraft.NBT;
using Minecraft.Utils;
using SixLabors.ImageSharp.PixelFormats;

namespace Minecraft.Regions;

public static class ChunkSectionParser
{
    private const string BiomesString = "biomes";

    public static ChunkSection? ParseSection(NbtStream stream)
    {
        var type = stream.GetTagType();

        while (type != TagType.End)
        {
            var nameLength = stream.GetUInt16();
            stream.Skip(nameLength);

            // Only compound child tags are "biomes" and "block_states"
            if (type == TagType.Compound && nameLength > BiomesString.Length)
            {
                return GetChunkSection(stream);
            }

            stream.SkipTag(type);
            type = stream.GetTagType();
        }

        return null;
    }

    private static ChunkSection? GetChunkSection(NbtStream stream)
    {
        var palette = Array.Empty<Rgba32>();
        var blockStates = Array.Empty<long>();

        var type = stream.GetTagType();

        while (type != TagType.End)
        {
            stream.Skip(stream.GetUInt16()); // Skip tag name

            if (type == TagType.LongArray)
            {
                var length = stream.GetInt32();
                blockStates = new long[length];

                for (var i = 0; i < length; i++)
                    blockStates[i] = stream.GetInt64();
            }
            else if (type == TagType.List)
            {
                palette = GetPalette(stream);
                if (palette.Length <= 1 && palette.All(b => b.A == 0))
                {
                    return null;
                }
            }

            type = stream.GetTagType();
        }

        return ChunkSection.FromStatesAndPalette(blockStates, palette);
    }

    private static Rgba32[] GetPalette(NbtStream stream)
    {
        var childType = stream.GetTagType();
        var count = stream.GetInt32();

        if (childType != TagType.Compound || count <= 0)
        {
            return Array.Empty<Rgba32>();
        }

        var palette = new Rgba32[count];

        for (var i = 0; i < count; i++)
            palette[i] = GetPaletteBlockColor(stream);

        return palette;
    }

    private static Rgba32 GetPaletteBlockColor(NbtStream stream)
    {
        var result = Block.BlockColor.Rgba32Transparent;

        var type = stream.GetTagType();

        while (type != TagType.End)
        {
            stream.Skip(stream.GetUInt16()); // Skip name

            if (type == TagType.String) // "Name" is only relevant field and only field of type TagType.String
            {
                result = Block.GetBlockColor(stream.GetString());
            }
            else
            {
                stream.SkipTag(type);
            }

            type = stream.GetTagType();
        }

        return result;
    }
}
