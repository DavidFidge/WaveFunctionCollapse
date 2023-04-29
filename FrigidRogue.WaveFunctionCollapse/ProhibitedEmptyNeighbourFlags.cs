namespace FrigidRogue.WaveFunctionCollapse;

[Flags]
public enum ProhibitedEmptyNeighbourFlags
{
    None = 0,
    Uncollapsed = 1,
    Unused = 2,
    EdgeOfMap = 4,
    All = 7
}