﻿using System.Collections;

namespace Minecraft.NBT;

public class ByteArrayTag : Tag, IEnumerable<byte>
{
    public byte[] Data { get; set; }

    public int Length => Data.Length;

    public ByteArrayTag(byte[] data)
        : base(TagType.ByteArray)
    {
        Data = data;
    }

    public static ByteArrayTag FromStream(Stream s)
    {
        var length = s.GetInt32();

        return new ByteArrayTag(s.GetBytes(length));
    }

    public override ByteArrayTag ToByteArrayTag() => this;

    public static implicit operator ByteArrayTag(byte[] bytes) => new(bytes);

    public static implicit operator byte[](ByteArrayTag tag) => tag.Data;

    public IEnumerator<byte> GetEnumerator() => Data.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
}