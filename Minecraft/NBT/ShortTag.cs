using Minecraft.Utils;

namespace Minecraft.NBT;

public class ShortTag : Tag
{
    public short Data { get; }

    public ShortTag(short data)
        : base(TagType.Short)
    {
        Data = data;
    }

    public static ShortTag FromStream(NbtStream s) => new(s.GetInt16());

    public static void SkipInStream(NbtStream s) => s.Skip(2);

    public override ShortTag ToShortTag() => this;

    public override IntTag ToIntTag() => new(Data);

    public override LongTag ToLongTag() => new(Data);

    public static implicit operator short(ShortTag tag) => tag.Data;

    public static implicit operator int(ShortTag tag) => tag.Data;

    public static implicit operator long(ShortTag tag) => tag.Data;
}
