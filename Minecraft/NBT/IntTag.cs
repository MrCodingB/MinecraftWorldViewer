namespace Minecraft.NBT;

public class IntTag : Tag
{
    public int Data { get; set; }

    public IntTag(int data)
        : base(TagType.Int)
    {
        Data = data;
    }

    public static IntTag FromStream(Stream s) => new(s.GetInt32());

    public override IntTag ToIntTag() => this;

    public override LongTag ToLongTag() => new(Data);

    public static implicit operator IntTag(byte b) => new(b);

    public static implicit operator IntTag(short s) => new(s);

    public static implicit operator IntTag(int i) => new(i);

    public static implicit operator int(IntTag tag) => tag.Data;

    public static implicit operator long(IntTag tag) => tag.Data;
}
