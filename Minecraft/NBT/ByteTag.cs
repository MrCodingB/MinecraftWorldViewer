namespace Minecraft.NBT;

public class ByteTag : Tag
{
    public byte Data { get; set; }
    
    public ByteTag(byte data) 
        : base(TagType.Byte)
    {
        Data = data;
    }

    public static ByteTag FromStream(Stream s) => new(s.GetByte());

    public override ByteTag ToByteTag() => this;

    public override ShortTag ToShortTag() => new(Data);

    public override IntTag ToIntTag() => new(Data);

    public override LongTag ToLongTag() => new(Data);

    public static implicit operator ByteTag(byte b) => new(b);

    public static implicit operator byte(ByteTag tag) => tag.Data;

    public static implicit operator short(ByteTag tag) => tag.Data;

    public static implicit operator int(ByteTag tag) => tag.Data;

    public static implicit operator long(ByteTag tag) => tag.Data;
}
