using Minecraft.Utils;

namespace Minecraft.NBT;

public class DoubleTag : Tag
{
    public double Data { get; }
    
    public DoubleTag(double data) 
        : base(TagType.Double)
    {
        Data = data;
    }

    public static DoubleTag FromStream(NbtStream s) => new(s.GetDouble());

    public static void SkipInStream(NbtStream s) => s.Skip(8);

    public override DoubleTag ToDoubleTag() => this;

    public static implicit operator double(DoubleTag tag) => tag.Data;
}
