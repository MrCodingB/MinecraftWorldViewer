namespace MinecraftApi.NBT;

public class NamedTag : DataTag
{
    public string Name { get; }

    public NamedTag(TagType type, string name)
        : base(type)
    {
        Name = name;
    }
}
