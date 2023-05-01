namespace FrigidRogue.WaveFunctionCollapse;

public enum EntropyHeuristic
{
    ReduceByCountOfNeighbours,
    ReduceByWeightOfNeighbours,
    ReduceByMaxWeightOfNeighbours,
    ReduceByCountAndWeightOfNeighbours,
    ReduceByCountAndMaxWeightOfNeighbours,
    ReduceByCountOfAllTilesMinusPossibleTiles
}