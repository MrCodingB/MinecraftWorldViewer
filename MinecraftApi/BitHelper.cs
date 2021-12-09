namespace MinecraftApi
{
    // Big-endian bit converter to have platform independent parsing
    public static class BitHelper
    {
        public static ushort ToUInt16(byte[] bytes)
            => (ushort)((bytes[0] << 8) | bytes[1]);

        public static short ToInt16(byte[] bytes)
            => (short)((bytes[0] << 8) | bytes[1]);

        public static int ToInt24(byte[] bytes)
            => (bytes[0] << 16) | (bytes[1] << 8) | bytes[2];

        public static int ToInt32(byte[] bytes)
            => (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];

        public static long ToInt64(byte[] bytes)
            => ((long)bytes[0] << 56) | ((long)bytes[1] << 48) | ((long)bytes[2] << 40) | ((long)bytes[3] << 32) |
               ((long)bytes[4] << 24) | ((long)bytes[5] << 16) | ((long)bytes[6] << 8) | bytes[7];

        public static float ToFloat(byte[] bytes)
            => BitConverter.ToSingle(BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes);

        public static double ToDouble(byte[] bytes)
            => BitConverter.ToDouble(BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes);
    }
}
