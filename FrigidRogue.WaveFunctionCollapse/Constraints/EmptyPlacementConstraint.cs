using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class EmptyPlacementConstraint : TileConstraint
{
    public override int Order => 5;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices)
    {
        var directionsThatMustBeNonEmpty = tileToCheck.EmptyPlacementRules
            .Where(r => r.Value == false)
            .Select(e => e.Key);

        foreach (var direction in directionsThatMustBeNonEmpty)
        {
            var neighbour =
                tile.Neighbours.FirstOrDefault(n => Direction.GetDirection(tile.Point, n.Point) == direction);

            if (neighbour == null || neighbour.IsUnused || !neighbour.IsCollapsed)
                return false;
        }

        return true;
    }
}