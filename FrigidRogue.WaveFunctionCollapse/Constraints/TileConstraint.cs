﻿namespace FrigidRogue.WaveFunctionCollapse;

public abstract class TileConstraint : ITileConstraint
{
    public abstract int Order { get; }

    public virtual void Initialise(List<TileContent> tileContent)
    {
    }

    public abstract bool Check(TileResult tile, TileChoice tileToCheck, HashSet<TileChoice> otherChoices);

    public virtual void Revert(TileResult tile, TileChoice tileToCheck)
    {
    }

    public virtual void AfterChoice(TileResult tile, TileChoice tileToCheck)
    {
    }
}