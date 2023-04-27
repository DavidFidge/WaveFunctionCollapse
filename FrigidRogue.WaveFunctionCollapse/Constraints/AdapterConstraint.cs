namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class AdapterConstraint : TileConstraint
{
    public override int Order => 1;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices)
    {
        var canAdaptTo = tile.Neighbours
            .Where(t => t.IsCollapsed)
            .All(t => tileToCheck.CanAdaptTo(tile.Point, t));

        return canAdaptTo;
    }
}