using Minecraft.NBT;
using Minecraft.Utils;
using SixLabors.ImageSharp.PixelFormats;

namespace Minecraft.Regions;

public static class ChunkSectionParser
{
    private const string BiomesString = "biomes";

    public static ChunkSection? ParseSection(NbtStream stream, out sbyte y)
    {
        var type = stream.GetTagType();

        ChunkSection? section = null;
        y = -128;

        while (type != TagType.End)
        {
            var nameLength = stream.GetUInt16();
            stream.Skip(nameLength);

            // Only compound child tags are "biomes" and "block_states"
            if (type == TagType.Compound && nameLength > BiomesString.Length)
            {
                section = GetChunkSection(stream);
            }
            else if (type == TagType.Byte)
            {
                y = (sbyte)stream.GetByte();
            }
            else
            {
                stream.SkipTag(type);
            }

            type = stream.GetTagType();
        }

        return section;
    }

    private static ChunkSection? GetChunkSection(NbtStream stream)
    {
        var palette = Array.Empty<Rgba32>();
        var blockStates = Array.Empty<long>();

        var type = stream.GetTagType();

        while (type != TagType.End)
        {
            stream.SkipTag(TagType.String); // Skip tag name

            if (type == TagType.LongArray)
            {
                blockStates = stream.GetLongArray();
            }
            else if (type == TagType.List)
            {
                palette = GetPalette(stream);
            }

            type = stream.GetTagType();
        }

        if (palette.Length <= 1 && palette.All(b => b.A == 0))
        {
            return null;
        }

        return ChunkSection.FromStatesAndPalette(blockStates, palette);
    }

    private static Rgba32[] GetPalette(NbtStream stream)
    {
        var childType = stream.GetTagType();
        var count = stream.GetInt32();

        if (count <= 0)
        {
            return Array.Empty<Rgba32>();
        }

        if (childType != TagType.Compound) 
        {
            throw new InvalidOperationException("Invalid child type for palette found");
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
                var name = stream.GetString();
                result = Block.GetBlockColor(name);
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
