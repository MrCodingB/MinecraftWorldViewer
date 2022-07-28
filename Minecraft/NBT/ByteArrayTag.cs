using System.Collections;
using Minecraft.Utils;

namespace Minecraft.NBT;

public class ByteArrayTag : Tag, IEnumerable<byte>
{
    public byte[] Data { get; }

    public int Length => Data.Length;

    public ByteArrayTag(byte[] data)
        : base(TagType.ByteArray)
    {
        Data = data;
    }

    public static ByteArrayTag FromStream(NbtStream s)
    {
        var length = s.GetInt32();

        return new ByteArrayTag(s.GetBytes(length));
    }

    public static void SkipInStream(NbtStream s) => s.Skip(s.GetInt32());

    public override ByteArrayTag ToByteArrayTag() => this;

    public static implicit operator byte[](ByteArrayTag tag) => tag.Data;

    public IEnumerator<byte> GetEnumerator() => Data.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
}
