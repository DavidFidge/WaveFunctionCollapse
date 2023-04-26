using FrigidRogue.WaveFunctionCollapse.Options;

namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public class PlacementConstraint : TileConstraint
{
    private int _mapWidth;
    private int _mapHeight;
    public override int Order => 4;

    public override void Initialise(List<TileContent> tileContent, MapOptions mapOptions,
        GeneratorOptions generatorOptions)
    {
        _mapWidth = mapOptions.MapWidth;
        _mapHeight = mapOptions.MapHeight;
    }

    public override bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> otherChoices)
    {
        var passesPlacementRule = tileToCheck.TileContent.PassesPlacementRule(tile.Point, _mapWidth, _mapHeight);
        
        return passesPlacementRule;
    }
}