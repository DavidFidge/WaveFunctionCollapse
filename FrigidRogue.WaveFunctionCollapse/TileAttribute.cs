﻿namespace FrigidRogue.WaveFunctionCollapse;

public class TileAttribute
{
    public string Symmetry { get; set; }
    public int Weight { get; set; } = 1;
    public string Adapters { get; set; }
    public string MandatoryAdapters { get; set; }
    public string InitialisationRule { get; set; }
    public string PlacementRule { get; set; }
    public int Limit { get; set; } = -1;
    public bool CanExceedLimitIfOnlyValidTile { get; set; } = false;
    public bool FlipHorizontally { get; set; }
    public bool FlipVertically { get; set; }
}