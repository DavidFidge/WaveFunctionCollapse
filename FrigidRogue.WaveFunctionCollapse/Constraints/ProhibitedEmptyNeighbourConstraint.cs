using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class ProhibitedEmptyNeighbourConstraint : TileConstraint
{
    public override int Order => 5;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices)
    {
        var directionsThatMustBeNonEmpty = tileToCheck.ProhibitedEmptyNeighbours
            .Where(r => r.Value != ProhibitedEmptyNeighbourFlags.None);

        foreach (var direction in directionsThatMustBeNonEmpty)
        {
            var neighbour =
                tile.Neighbours.FirstOrDefault(n => Direction.GetDirection(tile.Point, n.Point) == direction.Key);

            if (neighbour == null)
            {
                if (direction.Value.HasFlag(ProhibitedEmptyNeighbourFlags.EdgeOfMap))
                    return false;

                continue;
            }

            if (neighbour.IsUnused)
            {
                if (direction.Value.HasFlag(ProhibitedEmptyNeighbourFlags.Unused))
                    return false;

                continue;
            }

            if (!neighbour.IsCollapsed)
            {
                if (direction.Value.HasFlag(ProhibitedEmptyNeighbourFlags.Uncollapsed))
                    return false;
            }
        }

        return true;
    }
}