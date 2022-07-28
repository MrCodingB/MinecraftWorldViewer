namespace Minecraft.Utils;

public static class StreamUtil
{
    public static byte[] GetBytes(this Stream stream, int count)
    {
        var bytes = new byte[count];

        if (stream.Read(bytes, 0, count) < count)
        {
            throw new EndOfStreamException();
        }

        return bytes;
    }
}
