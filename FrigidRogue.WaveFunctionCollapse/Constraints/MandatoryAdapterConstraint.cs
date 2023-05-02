namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class MandatoryAdapterConstraint : TileConstraint
{
    public override int Order => 3;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices)
    {
        if (string.IsNullOrEmpty(tileToCheck.MandatoryAdapter.Pattern))
            return true;

        foreach (var neighbour in tile.Neighbours.Where(n => n.IsCollapsed))
        {
            if (tileToCheck.IsAdapterMandatory(tile.Point, neighbour))
            {
                return true;
            }
        }

        return false;
    }
}