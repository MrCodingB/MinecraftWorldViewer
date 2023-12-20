using Minecraft.NBT;
using Minecraft.Utils;

namespace Minecraft.Regions;

public sealed class ChunkParser
{
    private const byte Utf8CharCodeH = 72;

    private static readonly long LevelLongHash = BitHelper.StringToInt64("Level");
    private static readonly long XPosLongHash = BitHelper.StringToInt64("xPos");
    private static readonly long ZPosLongHash = BitHelper.StringToInt64("zPos");
    private static readonly long SectionsLongHash = BitHelper.StringToInt64("sections");

    private static readonly int MaxRelevantNameLength = "Heightmaps".Length;

    private ChunkSection[]? Sections { get; set; }

    private int? XPos { get; set; }

    private int? ZPos { get; set; }

    private ushort[]? Heightmap { get; set; }

    public Chunk? ParseChunk(NbtStream stream)
    {
        Sections = null;
        XPos = null;
        ZPos = null;
        Heightmap = null;

        return GetChunkFromStream(stream);
    }

    private Chunk? GetChunkFromStream(NbtStream stream)
    {
        var type = stream.GetTagType();

        while (type != TagType.End)
        {
            var nameLength = stream.GetUInt16();
            if (nameLength <= MaxRelevantNameLength)
            {
                var chunk = TestForChunkValues(stream, type, nameLength);
                if (chunk is not null)
                {
                    return chunk;
                }
            }
            else
            {
                stream.Skip(nameLength);
                stream.SkipTag(type);
            }

            type = stream.GetTagType();
        }

        return null;
    }

    private Chunk? TestForChunkValues(NbtStream stream, TagType type, ushort nameLength)
    {
        var nameBytes = stream.GetBytes(nameLength);
        var nameLongValue = BitHelper.BytesToInt64(nameBytes);

        if (nameLongValue == LevelLongHash)
        {
            return GetChunkFromStream(stream);
        }

        if (nameLongValue == XPosLongHash)
        {
            XPos = stream.GetInt32();
        }
        else if (nameLongValue == ZPosLongHash)
        {
            ZPos = stream.GetInt32();
        }
        else if (nameLongValue == SectionsLongHash)
        {
            Sections = GetChunkSectionsFromStream(stream);
        }
        else if (type == TagType.Compound && nameBytes[0] == Utf8CharCodeH)
        {
            Heightmap = ChunkHeightmapParser.GetChunkHeightmapFromStream(stream);
        }
        else
        {
            stream.SkipTag(type);
            return null;
        }

        if (XPos.HasValue && ZPos.HasValue && Sections is not null && Heightmap is not null)
        {
            return new Chunk(XPos.Value, ZPos.Value, Sections, Heightmap);
        }

        return null;
    }

    private static ChunkSection[] GetChunkSectionsFromStream(NbtStream stream)
    {
        var sections = new ChunkSection[25];

        var listChildTagType = stream.GetTagType();
        var count = stream.GetInt32();

        if (listChildTagType == TagType.End || count <= 0)
        {
            return sections;
        }

        var parsedSections = 0;
        while (parsedSections < count)
        {
            parsedSections++;
            if (parsedSections > sections.Length)
            {
                stream.SkipTag(listChildTagType);
                continue;
            }

            var section = ChunkSectionParser.ParseSection(stream, out var y);

            // Sometimes a section is saved that is below the world-limit (i.e. at y < -4)
            // These sections are ignored, to achieve consistency between
            // array-index and y-level of each section
            var i = y + 4;

            if (section is not null && 0 <= i && i < sections.Length)
            {
                sections[i] = section;
            }
        }

        return sections;
    }
}
