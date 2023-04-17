using FrigidRogue.MonoGame.Core.Extensions;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse;

public class TileResult
{
    public List<TileResult> Neighbours { get; private set; }
    public Point Point { get; }
    public int Index { get; }
    public bool IsCollapsed => TileChoice != null;
    public TileChoice TileChoice { get; set; }
    public int Entropy { get; set; } = Int32.MaxValue;

    public TileResult(Point point, int maxWidth)
    {
        Point = point;
        Index = point.ToIndex(maxWidth);
    }

    public void SetNeighbours(TileResult[] tiles, int maxWidth, int maxHeight)
    {
        Neighbours = Point
            .Neighbours(maxWidth - 1, maxHeight - 1, AdjacencyRule.Types.Cardinals)
            .Select(n => tiles[n.ToIndex(maxWidth)])
            .ToList();
    }

    public void SetTile(TileChoice tileChoice)
    {
        TileChoice = tileChoice;
    }
}