using System.IO.Compression;

namespace MapGenerator
{
    public class ChunkRawData
    {
        public int Length { get; init; }

        public CompressionType Compression { get; init; }

        public byte[] CompressedData { get; init; }

        public int DataLength => Length - 1;

        public Stream GetDataStream()
        {
            var stream = new MemoryStream(CompressedData);

            return Compression switch
            {
                CompressionType.GZip => new GZipStream(stream, CompressionMode.Decompress),
                CompressionType.Zlib => new ZLibStream(stream, CompressionMode.Decompress),
                _ => stream
            };
        }
    }
}
