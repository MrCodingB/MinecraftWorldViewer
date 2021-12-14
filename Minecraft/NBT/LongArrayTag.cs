using System.Collections;

namespace Minecraft.NBT;

public class LongArrayTag : Tag, IEnumerable<long>
{
    public long[] Data { get; set; }

    public int Length => Data.Length;

    public LongArrayTag(long[] data)
        : base(TagType.LongArray)
    {
        Data = data;
    }

    public static LongArrayTag FromStream(Stream s)
    {
        var length = s.GetInt32();

        var data = new long[length];
        var buffer = s.GetBytes(length * 8);

        var offset = 0;
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = BitHelper.ToInt64(buffer, offset);
            offset += 8;
        }

        return new LongArrayTag(data);
    }

    public override LongArrayTag ToLongArrayTag() => this;

    public static implicit operator LongArrayTag(long[] longs) => new(longs);

    public static implicit operator long[](LongArrayTag tag) => tag.Data;

    public IEnumerator<long> GetEnumerator() => Data.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
}
