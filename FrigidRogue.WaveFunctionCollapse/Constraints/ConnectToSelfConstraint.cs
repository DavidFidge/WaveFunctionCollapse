using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class ConnectToSelfConstraint : TileConstraint
{
    public override int Order => 7;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices)
    {
        foreach (var neighbour in tile.Neighbours.Where(n => n.IsCollapsed))
        {
            var direction = Direction.GetDirection(tile.Point, neighbour.Point);

            var canConnectToSelf = tileToCheck.CanConnectToSelf[direction];

            if (!canConnectToSelf && tileToCheck.TileTemplate.Name == neighbour.ChosenTile.TileTemplate.Name)
                return false;
        }

        return true;
    }
}