namespace Minecraft.NBT;

public class ShortTag : Tag
{
    public short Data { get; set; }
    
    public ShortTag(short data) 
        : base(TagType.Short)
    {
        Data = data;
    }

    public static ShortTag FromStream(Stream s) => new(s.GetInt16());
    
    public override ShortTag ToShortTag() => this;

    public override IntTag ToIntTag() => new(Data);

    public override LongTag ToLongTag() => new(Data);

    public static implicit operator ShortTag(byte b) => new(b);

    public static implicit operator ShortTag(short b) => new(b);
    
    public static implicit operator short(ShortTag tag) => tag.Data;

    public static implicit operator int(ShortTag tag) => tag.Data;

    public static implicit operator long(ShortTag tag) => tag.Data;
}
