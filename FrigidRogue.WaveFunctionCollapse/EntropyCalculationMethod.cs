namespace FrigidRogue.WaveFunctionCollapse;

public enum EntropyCalculationMethod
{
    ReduceByCountOfNeighbours,
    ReduceByWeightOfNeighbours,
    ReduceByCountAndMaxWeightOfNeighbours,
    ReduceByCountOfAllTilesMinusPossibleTiles
}