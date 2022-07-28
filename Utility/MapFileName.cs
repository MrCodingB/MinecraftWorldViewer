namespace Utility;

public static class MapFileName
{
    public static string Get(string mapName) => $"Map-{mapName}-{DateTime.Now.ToLocalTime():yyyy-MM-dd hh-mm-ss}.png";
}
