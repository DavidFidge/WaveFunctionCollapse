namespace FrigidRogue.WaveFunctionCollapse.Options;

public class GeneratorOptions : ICloneable
{
    public int FallbackAttempts = 5;
    public int FallbackRadius = 1;
    public int FallbackRadiusIncrement = 1;
    public Dictionary<string, string[]> PassMask = new();

    public Dictionary<int, string[]> PassMaskByPassIndex =>
        PassMask.ToDictionary(k => int.Parse(k.Key), k => k.Value);

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