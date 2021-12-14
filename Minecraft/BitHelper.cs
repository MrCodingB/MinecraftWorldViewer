namespace Minecraft;

// Big-endian bit converter to have platform independent parsing
public static class BitHelper
{
    public static ushort ToUInt16(byte[] bytes)
        => (ushort)((bytes[0] << 8) | bytes[1]);

    public static short ToInt16(byte[] bytes)
        => (short)((bytes[0] << 8) | bytes[1]);

    public static int ToInt24(byte[] bytes, int offset = 0)
        => (bytes[offset + 0] << 16) | (bytes[offset + 1] << 8) | bytes[offset + 2];

    public static int ToInt32(byte[] bytes)
        => ToInt32(bytes, 0);

    public static int ToInt32(byte[] bytes, int offset)
        => (bytes[offset + 0] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3];

    public static long ToInt64(byte[] bytes)
        => ToInt64(bytes, 0);

    public static long ToInt64(byte[] bytes, int offset)
        => ((long)bytes[0 + offset] << 56) | ((long)bytes[1 + offset] << 48) | ((long)bytes[2 + offset] << 40) |
           ((long)bytes[3 + offset] << 32) | ((long)bytes[4 + offset] << 24) | ((long)bytes[5 + offset] << 16) |
           ((long)bytes[6 + offset] << 8) | bytes[7 + offset];

    public static float ToFloat(byte[] bytes)
        => BitConverter.ToSingle(ReverseBytesIfLittleEndian(bytes));

    public static double ToDouble(byte[] bytes)
        => BitConverter.ToDouble(ReverseBytesIfLittleEndian(bytes));
    
    private static byte[] ReverseBytesIfLittleEndian(byte[] bytes)
        => BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes;
}
