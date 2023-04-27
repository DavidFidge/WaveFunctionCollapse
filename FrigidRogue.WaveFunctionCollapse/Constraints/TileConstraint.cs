using FrigidRogue.WaveFunctionCollapse.Options;

namespace FrigidRogue.WaveFunctionCollapse.Constraints;

public abstract class TileConstraint : ITileConstraint
{
    public abstract int Order { get; }

    public virtual void Initialise(List<TileTemplate> tileTemplates, MapOptions mapOptions)
    {
    }

    public abstract bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> allChoices);

    public virtual void Revert(TileChoice tileToCheck)
    {
    }

    public virtual void AfterChoice(TileChoice tileToCheck)
    {
    }
}