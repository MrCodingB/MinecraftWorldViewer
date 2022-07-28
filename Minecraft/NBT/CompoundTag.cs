using System.Collections;
using Minecraft.Utils;

namespace Minecraft.NBT;

public class CompoundTag : Tag, IDictionary<string, Tag>
{
    private IDictionary<string, Tag> Tags { get; }

    public int Count => Tags.Count;

    public bool IsReadOnly => Tags.IsReadOnly;

    public ICollection<string> Keys => Tags.Keys;

    public ICollection<Tag> Values => Tags.Values;

    public CompoundTag()
        : base(TagType.Compound)
    {
        Tags = new Dictionary<string, Tag>();
    }

    public static CompoundTag FromStream(NbtStream s)
    {
        var tag = new CompoundTag();

        var type = s.GetTagType();

        while (s.CanRead && type != TagType.End)
        {
            var name = s.GetString();
            tag[name] = s.GetTag(type);
            type = s.GetTagType();
        }

        return tag;
    }

    public static void SkipInStream(NbtStream s)
    {
        var type = s.GetTagType();

        while (s.CanRead && type != TagType.End)
        {
            s.Skip(s.GetUInt16()); // Skip tag name (string)
            s.SkipTag(type);
            type = s.GetTagType();
        }
    }

    public Tag this[string key]
    {
        get => Tags[key];
        set => Tags[key] = value;
    }

    public override CompoundTag ToCompoundTag() => this;

    public IEnumerator<KeyValuePair<string, Tag>> GetEnumerator() => Tags.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Tags.GetEnumerator();

    public void Add(KeyValuePair<string, Tag> item) => Tags.Add(item);

    public void Clear() => Tags.Clear();

    public bool Contains(KeyValuePair<string, Tag> item) => Tags.Contains(item);

    public void CopyTo(KeyValuePair<string, Tag>[] array, int arrayIndex) => Tags.CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<string, Tag> item) => Tags.Remove(item);

    public void Add(string key, Tag value) => Tags.Add(key, value);

    public bool ContainsKey(string key) => Tags.ContainsKey(key);

    public bool Remove(string key) => Tags.Remove(key);

    public bool TryGetValue(string key, out Tag value) => Tags.TryGetValue(key, out value!);
}
