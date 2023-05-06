namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class ConnectToSelfConstraint : TileConstraint
{
    public override int Order => 7;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices)
    {
        if (tileToCheck.TileTemplate.Attributes.CanConnectToSelf)
            return true;

        foreach (var neighbour in tile.Neighbours.Where(n => n.IsCollapsed))
        {
            if (tileToCheck.TileTemplate.Name == neighbour.ChosenTile.TileTemplate.Name)
            {
                return false;
            }
        }

        return true;
    }
}