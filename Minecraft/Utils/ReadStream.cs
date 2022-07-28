namespace Minecraft.Utils;

public sealed class ReadStream : Stream
{
    public const int DefaultInitialCapacity = 35840;

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => IntLength;

    public override long Position
    {
        get => IntLength;
        set => throw new NotSupportedException();
    }

    private byte[] Buffer = new byte[DefaultInitialCapacity];

    private int IntLength;

    private bool IsOpen = true;

    public NbtStream AsNbtStream() => new(Buffer, 0, IntLength);

    public override void Write(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        EnsureNotClosed();

        var newLength = IntLength + count;

        EnsureCapacity(newLength);

        if (count <= 8 && buffer != Buffer)
        {
            var i = offset;
            while (IntLength < newLength)
            {
                Buffer[IntLength++] = buffer[i];
                i++;
            }
        }
        else
        {
            System.Buffer.BlockCopy(buffer, offset, Buffer, IntLength, count);
            IntLength = newLength;
        }
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override void CopyTo(Stream destination, int bufferSize) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin loc) => throw new NotSupportedException();

    public override void SetLength(long value) => IntLength = (int)value;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            IsOpen = false;
    }

    private void EnsureNotClosed()
    {
        if (!IsOpen)
            throw new ObjectDisposedException(nameof(ReadStream));
    }

    private void EnsureCapacity(int value)
    {
        if (value <= Buffer.Length)
        {
            return;
        }

        var newCapacity = Math.Max(value, Buffer.Length * 2);
        var newBuffer = new byte[newCapacity];

        if (IntLength > 0)
        {
            System.Buffer.BlockCopy(Buffer, 0, newBuffer, 0, IntLength);
        }

        Buffer = newBuffer;
    }
}
