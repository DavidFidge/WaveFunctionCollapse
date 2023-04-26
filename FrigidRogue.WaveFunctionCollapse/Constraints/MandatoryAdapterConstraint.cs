namespace FrigidRogue.WaveFunctionCollapse;

public class MandatoryAdapterConstraint : TileConstraint
{
    public override bool Check(TileResult tile, TileChoice tileToCheck)
    {
        if (!tileToCheck.MandatoryAdapters.Any())
            return true;

        // Look through the neighbours of the chosen tile and remove any that don't have a mandatory adapter.
        foreach (var neighbour in tile.Neighbours.Where(n => n.IsCollapsed))
        {
            if (tileToCheck.IsAdapterMandatory(tile.Point, neighbour))
            {
                return true;
            }
        }

        tileToCheck.FailedConstraints.Add(this);

        return false;
    }
}