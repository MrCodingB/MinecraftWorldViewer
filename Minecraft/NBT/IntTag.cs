using Minecraft.Utils;

namespace Minecraft.NBT;

public class IntTag : Tag
{
    public int Data { get; }

    public IntTag(int data)
        : base(TagType.Int)
    {
        Data = data;
    }

    public static IntTag FromStream(NbtStream s) => new(s.GetInt32());

    public static void SkipInStream(NbtStream s) => s.Skip(4);

    public override IntTag ToIntTag() => this;

    public override LongTag ToLongTag() => new(Data);

    public static implicit operator int(IntTag tag) => tag.Data;

    public static implicit operator long(IntTag tag) => tag.Data;
}
