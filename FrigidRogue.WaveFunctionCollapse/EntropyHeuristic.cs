namespace FrigidRogue.WaveFunctionCollapse;

public enum EntropyHeuristic
{
    ReduceByCountOfNeighbours,
    ReduceByWeightOfNeighbours,
    ReduceByCountAndWeightOfNeighbours,
    ReduceByCountAndMaxWeightOfNeighbours,
    ReduceByCountOfAllTilesMinusPossibleTiles
}