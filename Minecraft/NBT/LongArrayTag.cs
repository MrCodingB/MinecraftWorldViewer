using System.Collections;
using Minecraft.Utils;

namespace Minecraft.NBT;

public class LongArrayTag : Tag, IEnumerable<long>
{
    public long[] Data { get; }

    public int Length => Data.Length;

    public LongArrayTag(long[] data)
        : base(TagType.LongArray)
    {
        Data = data;
    }

    public static LongArrayTag FromStream(NbtStream s)
    {
        var length = s.GetInt32();

        var data = new long[length];

        for (var i = 0; i < data.Length; i++)
        {
            data[i] = s.GetInt64();
        }

        return new LongArrayTag(data);
    }

    public static void SkipInStream(NbtStream s) => s.Skip(s.GetInt32() * 8);

    public override LongArrayTag ToLongArrayTag() => this;

    public static implicit operator long[](LongArrayTag tag) => tag.Data;

    public IEnumerator<long> GetEnumerator() => Data.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
}
