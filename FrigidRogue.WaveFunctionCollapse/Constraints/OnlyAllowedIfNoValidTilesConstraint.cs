namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class OnlyAllowedIfNoValidTilesConstraint : TileConstraint
{
    public override int Order => Int32.MaxValue;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> otherChoices)
    {
        if (!tileToCheck.TileContent.Attributes.OnlyAllowedIfNoValidTiles)
            return true;
        
        if (otherChoices.Any(t => !t.TileContent.Attributes.OnlyAllowedIfNoValidTiles))
            return false;

        return true;
    }
}