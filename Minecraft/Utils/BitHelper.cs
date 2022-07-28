using System.Text;

namespace Minecraft.Utils;

// Big-endian bit converter to have platform independent parsing
public static class BitHelper
{
    public static int ToInt24(byte[] bytes, long offset = 0)
        => (bytes[offset] << 16) | (bytes[offset + 1] << 8) | bytes[offset + 2];

    public static int ToInt32(byte[] bytes, long offset = 0)
        => (bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3];

    public static long ToInt64(byte[] bytes, long offset = 0)
        => ((long)((bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3]) << 32) |
           (uint)((bytes[offset + 4] << 24) | (bytes[offset + 5] << 16) | (bytes[offset + 6] << 8) | bytes[offset + 7]);

    public static float ToFloat(byte[] bytes, int offset = 0)
        => BitConverter.ToSingle(ReverseBytesIfLittleEndian(bytes, offset, 4));

    public static double ToDouble(byte[] bytes, int offset = 0)
        => BitConverter.ToDouble(ReverseBytesIfLittleEndian(bytes, offset, 8));

    /// <summary>
    /// The maximum length of src is sizeof(long).
    /// If src is longer, the long value will consist of just the last 8 bytes.
    /// </summary>
    /// <returns>A big-endian numerical representation of src</returns>
    public static long StringToInt64(string src) => BytesToInt64(Encoding.UTF8.GetBytes(src));

    /// <summary>
    /// The maximum length of bytes is sizeof(long).
    /// If bytes is longer, the long value will consist of just the last 8 bytes.
    /// </summary>
    /// <returns>A big-endian numerical representation of bytes</returns>
    public static long BytesToInt64(byte[] bytes)
    {
        var result = 0L;

        foreach (var b in bytes)
        {
            result <<= 8;
            result |= b;
        }

        return result;
    }

    private static byte[] ReverseBytesIfLittleEndian(byte[] bytes, int offset, int count)
    {
        var newBytes = new byte[count];

        if (!BitConverter.IsLittleEndian)
        {
            for (int i = 0, j = offset; i < count; i++, j++)
                newBytes[i] = bytes[j];
        }
        else
        {
            var j = offset + count;
            for (var i = 0; i < count; i++)
                newBytes[i] = bytes[--j];
        }

        return newBytes;
    }
}
