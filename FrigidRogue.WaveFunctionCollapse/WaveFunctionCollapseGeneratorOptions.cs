namespace FrigidRogue.WaveFunctionCollapse;

public class WaveFunctionCollapseGeneratorOptions : ICloneable
{
    public int MapWidth = 30;
    public int MapHeight = 30;

    public int FallbackAttempts = 5;
    public int FallbackRadius = 1;
    public int FallbackRadiusIncrement = 1;
    public EntropyHeuristic EntropyHeuristic = EntropyHeuristic.ReduceByCountOfAllTilesMinusPossibleTiles;

    public WaveFunctionCollapseGeneratorOptions()
    {
    }
    
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