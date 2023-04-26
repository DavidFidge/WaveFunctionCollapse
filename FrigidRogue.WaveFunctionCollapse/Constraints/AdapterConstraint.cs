namespace FrigidRogue.WaveFunctionCollapse;

public class AdapterConstraint : TileConstraint
{
    public override bool Check(TileResult tile, TileChoice tileToCheck)
    {
        var canAdaptTo = tile.Neighbours
            .Where(t => t.IsCollapsed)
            .All(t => tileToCheck.CanAdaptTo(tile.Point, t));

        if (!canAdaptTo)
            tileToCheck.FailedConstraints.Add(this);

        return canAdaptTo;
    }
}