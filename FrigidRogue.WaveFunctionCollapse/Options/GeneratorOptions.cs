namespace FrigidRogue.WaveFunctionCollapse.Options;

public class GeneratorOptions : ICloneable
{
    public int FallbackAttempts = 5;
    public int FallbackRadius = 1;
    public int FallbackRadiusIncrement = 1;
    public Dictionary<int, string[]> LayerMask = new();

    public EntropyHeuristic EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours;

    public GeneratorOptions()
    {
    }

    object ICloneable.Clone()
    {
        return MemberwiseClone();
    }

    public GeneratorOptions Clone()
    {
        return (GeneratorOptions)((ICloneable)this).Clone();
    }
}