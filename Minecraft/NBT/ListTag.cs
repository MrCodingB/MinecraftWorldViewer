using System.Collections;
using Minecraft.Utils;

namespace Minecraft.NBT;

public class ListTag : Tag, IEnumerable<Tag>
{
    public int Count => Items.Length;

    public TagType ItemType { get; }

    private Tag[] Items { get; }

    public ListTag()
        : this(TagType.End, Array.Empty<Tag>())
    {
    }

    public ListTag(TagType itemType, Tag[] items)
        : base(TagType.List)
    {
        ItemType = itemType;
        Items = items;
    }

    public static ListTag FromStream(NbtStream s)
    {
        var childType = s.GetTagType();
        var count = s.GetInt32();

        if (childType == TagType.End || count <= 0)
        {
            return new ListTag();
        }

        var items = new Tag[count];

        for (var i = 0; i < count; i++)
            items[i] = s.GetTag(childType);

        return new ListTag(childType, items);
    }

    public static void SkipInStream(NbtStream s)
    {
        var childType = s.GetTagType();
        var count = s.GetInt32();

        if (childType == TagType.End || count <= 0)
        {
            return;
        }

        for (var i = 0; i < count; i++)
            s.SkipTag(childType);
    }

    public Tag this[int index] => Items[index];

    public override ListTag ToListTag() => this;

    public IEnumerator<Tag> GetEnumerator() => Items.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
}
