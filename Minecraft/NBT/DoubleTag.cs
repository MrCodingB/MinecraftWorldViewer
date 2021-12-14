namespace Minecraft.NBT;

public class DoubleTag : Tag
{
    public double Data { get; set; }
    
    public DoubleTag(double data) 
        : base(TagType.Double)
    {
        Data = data;
    }

    public static DoubleTag FromStream(Stream s) => new(s.GetDouble());

    public override DoubleTag ToDoubleTag() => this;

    public static implicit operator DoubleTag(float f) => new(f);

    public static implicit operator DoubleTag(double d) => new(d);

    public static implicit operator double(DoubleTag tag) => tag.Data;
}
