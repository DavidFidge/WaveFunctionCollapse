namespace FrigidRogue.WaveFunctionCollapse.Options;

public class MapOptions
{
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public int TileSizeMultiplier { get; set; } = 1;

    public MapOptions(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
    }
}