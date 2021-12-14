namespace Minecraft.NBT;

public class EndTag : Tag
{
    public EndTag() 
        : base(TagType.End)
    {
    }

    public static EndTag FromStream(Stream s) => new();

    public override EndTag ToEndTag() => this;
}
