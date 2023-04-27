using FrigidRogue.WaveFunctionCollapse.Options;

namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public interface ITileConstraint
{
    public int Order { get; }
    public void Initialise(List<TileTemplate> tileTemplates, MapOptions mapOptions);
    public bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices);
    public void Revert(TileChoice tileToCheck);
    public void AfterChoice(TileChoice tileToCheck);
}