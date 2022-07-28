using System.Text;
using Minecraft.NBT;

namespace Minecraft.Utils;

public class NbtStream : Stream
{
    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length => Buffer.LongLength;

    public override long Position
    {
        get => IntPosition;
        set => throw new NotSupportedException();
    }

    private readonly int IntLength;

    private readonly byte[] Buffer;

    private int IntPosition;

    public NbtStream(byte[] buffer, int index, int length)
    {
        Buffer = buffer;
        IntPosition = index;
        IntLength = length;
    }

    public NbtStream(byte[] buffer)
    {
        Buffer = buffer;
        IntLength = buffer.Length;
    }

    public byte GetByte()
    {
        if (IntPosition >= IntLength)
        {
            throw new EndOfStreamException();
        }

        return Buffer[IntPosition++];
    }

    public byte[] GetBytes(int count)
    {
        var bytes = new byte[count];

        if (Read(bytes, 0, count) != count)
        {
            throw new EndOfStreamException();
        }

        return bytes;
    }

    public ushort GetUInt16() => (ushort)((Buffer[IntPosition++] << 8) | Buffer[IntPosition++]);

    public short GetInt16() => (short)((Buffer[IntPosition++] << 8) | Buffer[IntPosition++]);

    public int GetInt32()
    {
        var result = BitHelper.ToInt32(Buffer, IntPosition);
        IntPosition += 4;
        return result;
    }

    public long GetInt64()
    {
        var result = BitHelper.ToInt64(Buffer, IntPosition);
        IntPosition += 8;
        return result;
    }

    public float GetFloat()
    {
        var result = BitHelper.ToFloat(Buffer, IntPosition);
        IntPosition += 4;
        return result;
    }

    public double GetDouble()
    {
        var result = BitHelper.ToDouble(Buffer, IntPosition);
        IntPosition += 8;
        return result;
    }

    public string GetString()
    {
        var length = GetUInt16();
        var str = Encoding.UTF8.GetString(Buffer, IntPosition, length);
        IntPosition += length;
        return str;
    }

    public TagType GetTagType() => (TagType)GetByte();

    public Tag GetTag(TagType type)
        => type switch
        {
            TagType.End => EndTag.FromStream(this),
            TagType.Byte => ByteTag.FromStream(this),
            TagType.Short => ShortTag.FromStream(this),
            TagType.Int => IntTag.FromStream(this),
            TagType.Long => LongTag.FromStream(this),
            TagType.Float => FloatTag.FromStream(this),
            TagType.Double => DoubleTag.FromStream(this),
            TagType.ByteArray => ByteArrayTag.FromStream(this),
            TagType.String => StringTag.FromStream(this),
            TagType.List => ListTag.FromStream(this),
            TagType.Compound => CompoundTag.FromStream(this),
            TagType.IntArray => IntArrayTag.FromStream(this),
            TagType.LongArray => LongArrayTag.FromStream(this),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"Invalid tag type {type}")
        };

    public void SkipTag(TagType type)
    {
        switch (type)
        {
            case TagType.End: EndTag.SkipInStream(this); break;
            case TagType.Byte: ByteTag.SkipInStream(this); break;
            case TagType.Short: ShortTag.SkipInStream(this); break;
            case TagType.Int: IntTag.SkipInStream(this); break;
            case TagType.Long: LongTag.SkipInStream(this); break;
            case TagType.Float: FloatTag.SkipInStream(this); break;
            case TagType.Double: DoubleTag.SkipInStream(this); break;
            case TagType.ByteArray: ByteArrayTag.SkipInStream(this); break;
            case TagType.String: StringTag.SkipInStream(this); break;
            case TagType.List: ListTag.SkipInStream(this); break;
            case TagType.Compound: CompoundTag.SkipInStream(this); break;
            case TagType.IntArray: IntArrayTag.SkipInStream(this); break;
            case TagType.LongArray: LongArrayTag.SkipInStream(this); break;
            default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void Skip(int count)
    {
        IntPosition += count;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);

        var n = IntLength - IntPosition;
        if (n > count)
            n = count;
        if (n <= 0)
            return 0;

        if (n <= 8)
        {
            var byteCount = n;
            while (--byteCount >= 0)
                buffer[offset + byteCount] = Buffer[IntPosition + byteCount];
        }
        else
            System.Buffer.BlockCopy(Buffer, IntPosition, buffer, offset, n);

        IntPosition += n;

        return n;
    }

    public override int ReadByte() => GetByte();

    public override long Seek(long offset, SeekOrigin origin)
    {
        var intOffset = (int)offset;

        var newPosition = origin switch
        {
            SeekOrigin.Begin => intOffset,
            SeekOrigin.Current => IntPosition + intOffset,
            SeekOrigin.End => IntLength - intOffset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
        };

        if (newPosition < IntPosition)
        {
            throw new NotSupportedException("Cannot seek before current position");
        }

        IntPosition = newPosition;

        return IntPosition;
    }

    public override void Flush() => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
