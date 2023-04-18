namespace FrigidRogue.WaveFunctionCollapse;

public class WaveFunctionCollapseGeneratorOptions : ICloneable
{
    public int MapWidth { get; private set; }
    public int MapHeight { get; private set; }

    public int FallbackAttempts = 5;
    public int FallbackRadius = 1;
    public int FallbackRadiusIncrement = 1;

    public WaveFunctionCollapseGeneratorOptions(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
    }

    object ICloneable.Clone()
    {
        return MemberwiseClone();
    }

    public WaveFunctionCollapseGeneratorOptions Clone()
    {
        return (WaveFunctionCollapseGeneratorOptions)((ICloneable)this).Clone();
    }
}