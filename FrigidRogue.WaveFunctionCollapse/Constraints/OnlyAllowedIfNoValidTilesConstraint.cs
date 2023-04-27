namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class OnlyAllowedIfNoValidTilesConstraint : TileConstraint
{
    public override int Order => Int32.MaxValue;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> otherChoices)
    {
        if (!tileToCheck.TileTemplate.Attributes.OnlyAllowedIfNoValidTiles)
            return true;
        
        if (otherChoices.Any(t => !t.TileTemplate.Attributes.OnlyAllowedIfNoValidTiles))
            return false;

        return true;
    }
}