using System.Collections;

namespace Minecraft.NBT;

public class ListTag : Tag, IList<Tag>
{
    public int Count => Items.Count;

    public bool IsReadOnly => Items.IsReadOnly;

    public TagType ItemType { get; }

    private IList<Tag> Items { get; }

    public ListTag(TagType itemType)
        : base(TagType.List)
    {
        ItemType = itemType;
        Items = new List<Tag>();
    }

    public static ListTag FromStream(Stream s)
    {
        var childType = s.GetTagType();
        var count = s.GetInt32();

        if (childType == TagType.End || count <= 0)
        {
            return new ListTag(TagType.End);
        }

        var list = new ListTag(childType);

        for (var i = 0; i < count; i++)
        {
            list.Add(s.GetTag(list.ItemType));
        }

        return list;
    }

    public Tag this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    public override ListTag ToListTag() => this;

    public IEnumerator<Tag> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

    public void Add(Tag item) => Items.Add(item);

    public void Clear() => Items.Clear();

    public bool Contains(Tag item) => Items.Contains(item);

    public void CopyTo(Tag[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

    public bool Remove(Tag item) => Items.Remove(item);
    public int IndexOf(Tag item) => Items.IndexOf(item);

    public void Insert(int index, Tag item) => Items.Insert(index, item);

    public void RemoveAt(int index) => Items.RemoveAt(index);
}
