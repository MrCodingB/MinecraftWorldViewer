using Minecraft.NBT;
using Minecraft.Utils;

namespace Minecraft.Regions;

public sealed class ChunkParser
{
    private const byte Utf8CharCodeH = 72;

    private static readonly long LevelLongHash = BitHelper.StringToInt64("Level");
    private static readonly long XPosLongHash = BitHelper.StringToInt64("xPos");
    private static readonly long YPosLongHash = BitHelper.StringToInt64("yPos");
    private static readonly long ZPosLongHash = BitHelper.StringToInt64("zPos");
    private static readonly long SectionsLongHash = BitHelper.StringToInt64("sections");

    private static readonly int MaxRelevantNameLength = "Heightmaps".Length;

    private IDictionary<sbyte, ChunkSection>? Sections { get; set; }

    private int? XPos { get; set; }

    private int? YPos { get; set; }

    private int? ZPos { get; set; }

    private ushort[]? Heightmap { get; set; }

    public Chunk? ParseChunk(NbtStream stream)
    {
        Sections = null;
        XPos = null;
        YPos = null;
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
        else if (nameLongValue == YPosLongHash)
        {
            YPos = stream.GetInt32();
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

        if (!XPos.HasValue || !YPos.HasValue || !ZPos.HasValue || Sections is null || Heightmap is null)
        {
            return null;
        }

        var sections = GetAdjustedChunkSections(Sections, YPos.Value);

        return new Chunk(XPos.Value, ZPos.Value, sections, Heightmap);
    }

    private static IDictionary<sbyte, ChunkSection> GetChunkSectionsFromStream(NbtStream stream)
    {
        var listChildTagType = stream.GetTagType();
        var count = stream.GetInt32();

        if (listChildTagType == TagType.End || count <= 0)
        {
            return new Dictionary<sbyte, ChunkSection>();
        }

        var sections = new Dictionary<sbyte, ChunkSection>(count);

        for (var i = 0; i < count; i++)
        {
            var section = ChunkSectionParser.ParseSection(stream, out var y);
            if (section is not null)
            {
                sections.Add(y, section);
            }
        }

        return sections;
    }

    private static ChunkSection[] GetAdjustedChunkSections(IDictionary<sbyte, ChunkSection> sections, int chunkYPos)
    {
        var adjustedSections = new ChunkSection[25];

        foreach (var (y, section) in sections)
        {
            var adjustedIndex = y - chunkYPos;
            if (adjustedIndex < 0 || adjustedIndex >= adjustedSections.Length)
            {
                continue;
            }

            adjustedSections[adjustedIndex] = section;
        }

        return adjustedSections;
    }
}
