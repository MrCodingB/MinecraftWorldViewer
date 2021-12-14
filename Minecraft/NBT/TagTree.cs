namespace Minecraft.NBT;

public class TagTree
{
    public CompoundTag Root { get; }

    public string Name { get; }

    public TagTree(CompoundTag root, string name)
    {
        Root = root;
        Name = name;
    }

    public static TagTree FromStream(Stream s)
    {
        var type = s.GetTagType();

        if (type != TagType.Compound)
        {
            throw new InvalidOperationException($"Root tag of tree must be of type {TagType.Compound}");
        }

        var name = s.GetString();

        var root = s.GetTag(type).ToCompoundTag();

        return new TagTree(root, name);
    }
}
