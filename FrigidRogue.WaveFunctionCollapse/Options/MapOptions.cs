namespace FrigidRogue.WaveFunctionCollapse.Options;

public class MapOptions
{
    public int MapWidth { get; set; } = 30;
    public int MapHeight { get; set; } = 30;
    public int TileSizeMultiplier { get; set; } = 4;

    public MapOptions(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
    }
}