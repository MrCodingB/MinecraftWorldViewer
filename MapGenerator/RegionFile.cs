namespace MapGenerator
{
    public class RegionFile
    {
        public string FileName { get; init; }
        
        public int X { get; init; }
        
        public int Z { get; init; }
        
        public ChunkHeaderInfo[] ChunkHeaderInfos { get; init; }

        public ChunkRawData[] ChunksRawData { get; init; }
    }
}
