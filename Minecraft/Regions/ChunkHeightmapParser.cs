using Minecraft.NBT;
using Minecraft.Utils;

namespace Minecraft.Regions;

public static class ChunkHeightmapParser
{
    private const string WorldSurfaceHeightmapName = "WORLD_SURFACE";

    private const int HeightmapValueMask = 0x1ff;

    // Each long contains 7 heightmap values
    private const int MaxIndexInLong = 6;

    // With each value being an unsigned 9-bit integer value
    private const int BitsPerHeightmapValue = 9;

    public static ushort[]? GetChunkHeightmapFromStream(NbtStream stream)
    {
        if (!SeekWorldSurfaceHeightmap(stream))
        {
            return null;
        }

        var heightmap = ParseWorldSurfaceHeightmap(stream);

        SkipToEnd(stream);

        return heightmap;
    }

    private static bool SeekWorldSurfaceHeightmap(NbtStream stream)
    {
        var type = stream.GetTagType();

        while (type is not TagType.End)
        {
            var nameLength = stream.GetUInt16();
            stream.Skip(nameLength);

            if (nameLength == WorldSurfaceHeightmapName.Length)
            {
                return true;
            }

            stream.SkipTag(type);

            type = stream.GetTagType();
        }

        return false;
    }

    private static ushort[] ParseWorldSurfaceHeightmap(NbtStream stream)
    {
        var heightmap = new ushort[256];

        var longs = stream.GetLongArray();
        var longIndex = 0;
        var valueIndex = 0;
        var l = longs[longIndex];

        for (var i = 0; i < heightmap.Length; i++)
        {
            heightmap[i] = (ushort)(l & HeightmapValueMask);

            valueIndex++;
            if (valueIndex > MaxIndexInLong)
            {
                l = longs[++longIndex];
                valueIndex = 0;
            }
            else
            {
                l >>= BitsPerHeightmapValue;
            }
        }

        return heightmap;
    }

    private static void SkipToEnd(NbtStream stream)
    {
        var type = stream.GetTagType();

        while (type != TagType.End)
        {
            var tagNameLength = stream.GetUInt16();
            stream.Skip(tagNameLength);
            stream.SkipTag(type);

            type = stream.GetTagType();
        }
    }
}
