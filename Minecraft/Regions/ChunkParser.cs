using Minecraft.NBT;
using Minecraft.Utils;

namespace Minecraft.Regions;

public class ChunkParser
{
    private static readonly long LevelLongHash = BitHelper.StringToInt64("Level");
    private static readonly long XPosLongHash = BitHelper.StringToInt64("xPos");
    private static readonly long ZPosLongHash = BitHelper.StringToInt64("zPos");
    private static readonly long SectionsLongHash = BitHelper.StringToInt64("sections");
    private static readonly int MaxRelevantNameLength = "sections".Length;

    private IList<ChunkSection>? Sections { get; set; }

    private int? XPos { get; set; }

    private int? ZPos { get; set; }

    public Chunk ParseChunk(NbtStream stream)
    {
        Sections = null;
        XPos = null;
        ZPos = null;

        return GetChunkFromStream(stream);
    }

    private Chunk GetChunkFromStream(NbtStream stream)
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

        if (!XPos.HasValue || !ZPos.HasValue)
        {
            throw new InvalidOperationException("Did not find required tags");
        }

        return new Chunk(XPos.Value, ZPos.Value, Sections ?? new List<ChunkSection>());
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
        else
        {
            stream.SkipTag(type);
        }

        if (Sections is not null && XPos.HasValue && ZPos.HasValue)
        {
            return new Chunk(XPos.Value, ZPos.Value, Sections);
        }

        return null;
    }

    private static List<ChunkSection> GetChunkSectionsFromStream(NbtStream stream)
    {
        var sections = new List<ChunkSection>(25);

        var listChildTagType = stream.GetTagType();
        var count = stream.GetInt32();

        if (listChildTagType == TagType.End || count <= 0)
        {
            return sections;
        }

        for (var i = 0; i < count; i++)
        {
            var section = ChunkSectionParser.ParseSection(stream);
            if (section is not null)
            {
                sections.Add(section);
            }
        }

        return sections;
    }
}
