using System.Text;

namespace MinecraftApi;

public static class StreamUtil
{
    public static byte GetByte(this Stream stream)
    {
        var result = stream.ReadByte();

        if (result < 0)
        {
            throw new EndOfStreamException();
        }

        return (byte)result;
    }

    public static byte Peek(this Stream stream)
    {
        var position = stream.Position;

        var result = stream.GetByte();

        stream.Position = position;
        
        return result;
    }

    public static byte[] Peek(this Stream stream, int count)
    {
        var position = stream.Position;

        var result = stream.GetBytes(count);

        stream.Position = position;
        
        return result;
    }

    public static byte[] GetBytes(this Stream stream, int count)
    {
        var bytes = new byte[count];

        var result = stream.Read(bytes, 0, count);

        if (result < count)
        {
            throw new EndOfStreamException();
        }

        return bytes;
    }

    public static ushort GetUInt16(this Stream stream)
        => BitHelper.ToUInt16(stream.GetBytes(2));

    public static short GetInt16(this Stream stream)
        => BitHelper.ToInt16(stream.GetBytes(2));
    
    public static int GetInt32(this Stream stream)
        => BitHelper.ToInt32(stream.GetBytes(4));
    
    public static long GetInt64(this Stream stream)
        => BitHelper.ToInt64(stream.GetBytes(8));
    
    public static float GetFloat(this Stream stream)
        => BitHelper.ToFloat(stream.GetBytes(4));
    
    public static double GetDouble(this Stream stream)
        => BitHelper.ToDouble(stream.GetBytes(8));

    public static string GetString(this Stream stream)
    {
        var length = stream.GetUInt16();

        return Encoding.UTF8.GetString(stream.GetBytes(length));
    }
}
