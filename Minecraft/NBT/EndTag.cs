using Minecraft.Utils;

namespace Minecraft.NBT;

public class EndTag : Tag
{
    public EndTag()
        : base(TagType.End)
    {
    }

    public static EndTag FromStream(NbtStream _) => new();

    public static void SkipInStream(NbtStream _)
    {
    }

    public override EndTag ToEndTag() => this;
}
