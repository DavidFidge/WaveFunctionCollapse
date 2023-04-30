namespace FrigidRogue.WaveFunctionCollapse.Options;

public class TileAttribute
{
    public string TextureName { get; set; }
    public string Symmetry { get; set; }
    public int Weight { get; set; } = 1;
    public string Adapters { get; set; }
    public string MandatoryAdapters { get; set; }
    public string PlacementRule { get; set; }
    public int Limit { get; set; } = -1;
    public bool OnlyAllowedIfNoValidTiles { get; set; }
    public bool FlipHorizontally { get; set; }
    public bool FlipVertically { get; set; }
    public string Category { get; set; }
    public string[] CanConnectToCategories { get; set; }
    public string ProhibitedEmptyNeighbourRules { get; set; }
}