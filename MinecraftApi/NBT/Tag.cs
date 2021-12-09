namespace MinecraftApi.NBT;

public class Tag
{
    public TagType Type { get; }

    public Tag(TagType type)
    {
        Type = type;
    }
}
