using System.Collections;

namespace Minecraft.NBT;

public class IntArrayTag : Tag, IEnumerable<int>
{
    public int[] Data { get; set; }

    public int Length => Data.Length;

    public IntArrayTag(int[] data)
        : base(TagType.IntArray)
    {
        Data = data;
    }

    public static IntArrayTag FromStream(Stream s)
    {
        var length = s.GetInt32();

        var data = new int[length];
        var buffer = s.GetBytes(length * 4);

        var offset = 0;
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = BitHelper.ToInt32(buffer, offset);
            offset += 4;
        }

        return new IntArrayTag(data);
    }

    public override IntArrayTag ToIntArrayTag() => this;

    public static implicit operator IntArrayTag(int[] integers) => new(integers);

    public static implicit operator int[](IntArrayTag tag) => tag.Data;

    public IEnumerator<int> GetEnumerator() => Data.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
}
