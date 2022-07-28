using Minecraft.Utils;

namespace Minecraft.NBT;

public class LongTag : Tag
{
    public long Data { get; }

    public LongTag(long data)
        : base(TagType.Long)
    {
        Data = data;
    }

    public static LongTag FromStream(NbtStream s) => new(s.GetInt64());

    public static void SkipInStream(NbtStream s) => s.Skip(8);

    public override LongTag ToLongTag() => this;

    public static implicit operator long(LongTag tag) => tag.Data;
}
