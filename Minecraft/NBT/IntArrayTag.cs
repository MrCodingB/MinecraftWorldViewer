using System.Collections;
using Minecraft.Utils;

namespace Minecraft.NBT;

public class IntArrayTag : Tag, IEnumerable<int>
{
    public int[] Data { get; }

    public int Length => Data.Length;

    public IntArrayTag(int[] data)
        : base(TagType.IntArray)
    {
        Data = data;
    }

    public static IntArrayTag FromStream(NbtStream s)
    {
        var length = s.GetInt32();

        var data = new int[length];

        for (var i = 0; i < data.Length; i++)
        {
            data[i] = s.GetInt32();
        }

        return new IntArrayTag(data);
    }

    public static void SkipInStream(NbtStream s) => s.Skip(s.GetInt32() * 4);

    public override IntArrayTag ToIntArrayTag() => this;

    public static implicit operator int[](IntArrayTag tag) => tag.Data;

    public IEnumerator<int> GetEnumerator() => Data.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
}
