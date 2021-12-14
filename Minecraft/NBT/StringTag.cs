namespace Minecraft.NBT;

public class StringTag : Tag
{
    public string Data { get; set; }

    public int Length => Data.Length;
    
    public StringTag(string data) 
        : base(TagType.String)
    {
        Data = data;
    }

    public static StringTag FromStream(Stream s) => new(s.GetString());

    public override StringTag ToStringTag() => this;

    public static implicit operator StringTag(string s) => new(s);

    public static implicit operator string(StringTag tag) => tag.Data;
}
