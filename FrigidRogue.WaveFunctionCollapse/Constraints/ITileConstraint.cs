namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public interface ITileConstraint
{
    public int Order { get; }
    public void Initialise(List<TileContent> tileContent);
    public bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> otherChoices);
    public void Revert(TileResult tile, TileChoice tileToCheck);
    public void AfterChoice(TileResult tile, TileChoice tileToCheck);
}