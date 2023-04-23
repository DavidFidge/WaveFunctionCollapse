namespace FrigidRogue.WaveFunctionCollapse;

public enum EntropyHeuristic
{
    ReduceByCountOfNeighbours,
    ReduceByWeightOfNeighbours,
    ReduceByCountAndMaxWeightOfNeighbours,
    ReduceByCountOfAllTilesMinusPossibleTiles
}