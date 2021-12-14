namespace Minecraft.NBT;

public class FloatTag : Tag
{
    public float Data { get; set; }
    
    public FloatTag(float data) 
        : base(TagType.Float)
    {
        Data = data;
    }

    public static FloatTag FromStream(Stream s) => new(s.GetFloat());

    public override FloatTag ToFloatTag() => this;

    public override DoubleTag ToDoubleTag() => new(Data);

    public static implicit operator FloatTag(float f) => new(f);

    public static implicit operator float(FloatTag tag) => tag.Data;

    public static implicit operator double(FloatTag tag) => tag.Data;
}
