namespace FrigidRogue.WaveFunctionCollapse.Options;

public class GeneratorOptions : ICloneable
{
    public int FallbackAttempts { get; set; } = 5;
    public int FallbackRadius { get; set; } = 1;
    public int FallbackRadiusIncrement { get; set; } = 1;
    public int SuccessfullyPlacedTilesToReduceFallbackRadius { get; set; } = -1;
    public Dictionary<string, string[]> PassMask { get; set; } = new();
    public string[] RunFirstRules { get; set; } = Array.Empty<string>();

    public Dictionary<int, string[]> PassMaskByPassIndex =>
        PassMask.ToDictionary(k => int.Parse(k.Key), k => k.Value);

    public EntropyHeuristic EntropyHeuristic = EntropyHeuristic.ReduceByCountOfNeighbours;

    object ICloneable.Clone()
    {
        return MemberwiseClone();
    }

    public GeneratorOptions Clone()
    {
        return (GeneratorOptions)((ICloneable)this).Clone();
    }
}