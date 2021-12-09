using MinecraftApi;

namespace MapGenerator
{
    public static class RegionFileHandler
    {
        public static async Task<RegionFile> GetRegionFile(DirectoryInfo regionFileDir, int x, int z)
        {
            var regionFileName = $"r.{x}.{z}.mca";

            var regionFileBytes = await GetRegionFileBytes(Path.Join(regionFileDir.FullName, regionFileName));

            var chunkHeaderInfos = GetChunkHeaderInfos(regionFileBytes);

            var chunksRawData = GetChunksRawData(regionFileBytes, chunkHeaderInfos);

            return new RegionFile
            {
                X = x,
                Z = z,
                FileName = regionFileName,
                ChunkHeaderInfos = chunkHeaderInfos,
                ChunksRawData = chunksRawData
            };
        }

        private static ChunkRawData[] GetChunksRawData(byte[] bytes, IEnumerable<ChunkHeaderInfo> chunkHeaderInfos)
        {
            var chunksRawData = new List<ChunkRawData>();

            foreach (var headerInfo in chunkHeaderInfos)
            {
                var index = headerInfo.Offset * 4096 - 1;

                var length = BitHelper.ToInt32(bytes[index..]);
                index += 4;

                var compressionType = (CompressionType)bytes[index];
                index += 1;

                var compressedData = bytes[index..(index + length - 1)];

                chunksRawData.Add(new ChunkRawData
                {
                    Length = length,
                    Compression = compressionType,
                    CompressedData = compressedData
                });
            }

            return chunksRawData.ToArray();
        }

        private static ChunkHeaderInfo[] GetChunkHeaderInfos(byte[] bytes)
        {
            var chunkHeaderInfos = new List<ChunkHeaderInfo>();

            var i = 0;

            while (i < 4096)
            {
                var info = new ChunkHeaderInfo
                {
                    Offset = BitHelper.ToInt24(bytes[i..]),
                    SectorCount = bytes[i + 3],
                    Timestamp = BitHelper.ToInt32(bytes[(i + 4096)..]) // Timestamps are 4kB later in the header
                };

                if (info.Offset > 1) // Minimal offset is 2 due to 8kB header
                {
                    chunkHeaderInfos.Add(info);
                }

                i += 4;
            }

            return chunkHeaderInfos.ToArray();
        }

        private static async Task<byte[]> GetRegionFileBytes(string path)
        {
            var bytes = await File.ReadAllBytesAsync(path);

            // Region files should always start with an 8kB header
            if (bytes.Length < 8192)
            {
                throw new ArgumentException("Region file at path is invalid: Header missing");
            }

            return bytes;
        }
    }
}
