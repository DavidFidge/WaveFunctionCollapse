namespace FrigidRogue.WaveFunctionCollapse;

public interface ITileConstraint
{ 
    public void Initialise(List<TileContent> tileContent);
    public bool Check(TileResult tile, TileChoice tileToCheck);
    public void Revert(TileResult tile, TileChoice tileToCheck);
    public void AfterChoice(TileResult tile, TileChoice tileToCheck);
}