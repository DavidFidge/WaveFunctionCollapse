namespace FrigidRogue.WaveFunctionCollapse;

public class OnlyAllowedIfNoValidTilesConstraint : TileConstraint
{
    public override int Order => Int32.MaxValue;

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> otherChoices)
    {
        if (otherChoices.Any(t => !t.TileContent.Attributes.OnlyAllowedIfNoValidTiles))
            return false;

        return true;
    }
}