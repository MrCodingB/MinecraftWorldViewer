using Minecraft.Utils;

namespace Minecraft.NBT;

public class FloatTag : Tag
{
    public float Data { get; }
    
    public FloatTag(float data) 
        : base(TagType.Float)
    {
        Data = data;
    }

    public static FloatTag FromStream(NbtStream s) => new(s.GetFloat());

    public static void SkipInStream(NbtStream s) => s.Skip(4);

    public override FloatTag ToFloatTag() => this;

    public override DoubleTag ToDoubleTag() => new(Data);

    public static implicit operator float(FloatTag tag) => tag.Data;

    public static implicit operator double(FloatTag tag) => tag.Data;
}
