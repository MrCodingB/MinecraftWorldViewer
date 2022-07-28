using Minecraft.Utils;

namespace Minecraft.NBT;

public class ByteTag : Tag
{
    public byte Data { get; }
    
    public ByteTag(byte data) 
        : base(TagType.Byte)
    {
        Data = data;
    }

    public static ByteTag FromStream(NbtStream s) => new(s.GetByte());

    public static void SkipInStream(NbtStream s) => s.Skip(1);

    public override ByteTag ToByteTag() => this;

    public override ShortTag ToShortTag() => new(Data);

    public override IntTag ToIntTag() => new(Data);

    public override LongTag ToLongTag() => new(Data);

    public static implicit operator byte(ByteTag tag) => tag.Data;

    public static implicit operator short(ByteTag tag) => tag.Data;

    public static implicit operator int(ByteTag tag) => tag.Data;

    public static implicit operator long(ByteTag tag) => tag.Data;
}
