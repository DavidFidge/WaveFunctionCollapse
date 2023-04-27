namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class CategoryConstraint : TileConstraint
{
    public override int Order => 4;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices)
    {
        var canAdaptTo = tile.Neighbours
            .Where(t => t.IsCollapsed)
            .All(t => tileToCheck.CanConnectToCategory(tile.Point, t));

        return canAdaptTo;
    }
}