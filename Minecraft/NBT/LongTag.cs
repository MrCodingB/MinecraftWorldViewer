namespace Minecraft.NBT;

public class LongTag : Tag
{
    public long Data { get; set; }
    
    public LongTag(long data) 
        : base(TagType.Long)
    {
        Data = data;
    }

    public static LongTag FromStream(Stream s) => new(s.GetInt64());
    
    public override LongTag ToLongTag() => this;

    public static implicit operator LongTag(byte b) => new(b);

    public static implicit operator LongTag(short b) => new(b);

    public static implicit operator LongTag(int b) => new(b);

    public static implicit operator LongTag(long b) => new(b);

    public static implicit operator long(LongTag tag) => tag.Data;
}
