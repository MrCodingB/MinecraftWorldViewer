namespace Minecraft.Regions;

public readonly struct ChunkHeader
{
    public readonly int Offset;

    public readonly int Length;

    public ChunkHeader(int sectorOffset, byte sectorCount)
    {
        Offset = sectorOffset * 4096;
        Length = sectorCount * 4096;
    }
}
