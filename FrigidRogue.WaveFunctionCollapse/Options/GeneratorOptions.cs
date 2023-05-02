using NCalc;

namespace FrigidRogue.WaveFunctionCollapse.Options;

public class GeneratorOptions : ICloneable
{
    public int FallbackAttempts { get; set; } = 5;
    public int FallbackRadius { get; set; } = 1;
    public int FallbackRadiusIncrement { get; set; } = 1;

    public int SuccessfullyPlacedTilesToReduceFallbackRadius { get; set; } = -1;
    public string SuccessfullyPlacedTilesToReduceFallbackRadiusFormula { get; set; }
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

    public void SetSuccessfullyPlacedTilesToReduceFallbackRadius(MapOptions options)
    {
        if (!string.IsNullOrEmpty(SuccessfullyPlacedTilesToReduceFallbackRadiusFormula))
        {
            var expression = new Expression(SuccessfullyPlacedTilesToReduceFallbackRadiusFormula);
            expression.Parameters["MaxX"] = options.MapWidth - 1;
            expression.Parameters["MaxY"] = options.MapHeight - 1;
            expression.Parameters["MapWidth"] = options.MapWidth;
            expression.Parameters["MapHeight"] = options.MapHeight;

            SuccessfullyPlacedTilesToReduceFallbackRadius = (int)expression.Evaluate();
        }
    }
}