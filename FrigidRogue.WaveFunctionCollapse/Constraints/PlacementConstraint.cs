using FrigidRogue.WaveFunctionCollapse.Options;

namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class PlacementConstraint : TileConstraint
{
    private int _mapWidth;
    private int _mapHeight;
    public override int Order => 4;

    public override void Initialise(List<TileTemplate> tileTemplates, MapOptions mapOptions)
    {
        _mapWidth = mapOptions.MapWidth;
        _mapHeight = mapOptions.MapHeight;
    }

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices)
    {
        var passesPlacementRule = tileToCheck.PassesPlacementRule(tile.Point, _mapWidth, _mapHeight);

        return passesPlacementRule;
    }
}