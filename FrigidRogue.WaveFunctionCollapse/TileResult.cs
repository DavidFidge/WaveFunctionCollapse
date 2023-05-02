using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.WaveFunctionCollapse.Options;
using NCalc;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse;

public class TileResult
{
    public List<TileResult> Neighbours { get; private set; }
    public Point Point { get; }

    public bool IsCollapsed => ChosenTile != null;
    public bool IsUnused { get; set; }
    public TileChoice ChosenTile { get; set; }
    public int Entropy { get; set; }
    public int StartingEntropy { get; set; }

    public TileResult(Point point)
    {
        Point = point;
    }

    public void SetNeighbours(TileResult[] tiles, int maxWidth, int maxHeight)
    {
        Neighbours = Point
            .Neighbours(maxWidth - 1, maxHeight - 1, AdjacencyRule.Types.Cardinals)
            .Select(n => tiles[n.ToIndex(maxWidth)])
            .ToList();
    }

    public bool IsWithinRule(string rule, Point point, MapOptions mapOptions)
    {
        if (String.IsNullOrEmpty(rule))
            return false;

        var expression = new Expression(rule);

        expression.Parameters["X"] = point.X;
        expression.Parameters["Y"] = point.Y;
        expression.Parameters["MaxX"] = mapOptions.MapWidth - 1;
        expression.Parameters["MaxY"] = mapOptions.MapHeight - 1;
        expression.Parameters["MapWidth"] = mapOptions.MapWidth;
        expression.Parameters["MapHeight"] = mapOptions.MapHeight;

        var isPointWithinEvaluationRule = (bool)expression.Evaluate();

        return isPointWithinEvaluationRule;
    }
}