using Minecraft.Utils;

namespace Minecraft.NBT;

public class StringTag : Tag
{
    public string Data { get; }

    public int Length => Data.Length;
    
    public StringTag(string data) 
        : base(TagType.String)
    {
        Data = data;
    }

    public static StringTag FromStream(NbtStream s) => new(s.GetString());

    public static void SkipInStream(NbtStream s) => s.Skip(s.GetUInt16());

    public override StringTag ToStringTag() => this;

    public static implicit operator string(StringTag tag) => tag.Data;
}
