namespace FrigidRogue.WaveFunctionCollapse.Options;

public class MapOptions
{
    public int MapWidth = 30;
    public int MapHeight = 30;
    public int TileSizeMultiplier = 4;

    public MapOptions()
    {
    }

    public MapOptions(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
    }
}