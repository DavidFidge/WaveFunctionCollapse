﻿using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse;

public class TileChoice
{
    public TileTemplate TileTemplate { get; }
    public SpriteEffects SpriteEffects { get; }
    public float Rotation { get; }
    public Dictionary<Direction, Adapter> Adapters { get; }
    public List<Adapter> MandatoryAdapters { get; }
    public int Weight => TileTemplate.Attributes.Weight;
    public Texture2D Texture => TileTemplate.Texture;

    public TileChoice(TileTemplate tileTemplate, Dictionary<Direction, Adapter> adapters, SpriteEffects spriteEffects = SpriteEffects.None, float rotation = 0f)
    {
        TileTemplate = tileTemplate;
        Adapters = adapters;
        SpriteEffects = spriteEffects;
        Rotation = rotation;

        if (string.IsNullOrEmpty(tileTemplate.Attributes.MandatoryAdapters))
            MandatoryAdapters = new List<Adapter>();
        else
        {
            MandatoryAdapters = tileTemplate.Attributes.MandatoryAdapters
                .Split(",")
                .Select(a => a.Trim())
                .Select(a => (Adapter)a)
                .ToList();
        }
    }

    public bool CanAdaptTo(Point point, TileResult neighbourTile)
    {
        var direction = Direction.GetCardinalDirection(point, neighbourTile.Point);

        var firstAdapter = Adapters[direction];
        var secondAdapter = neighbourTile.TileChoice.Adapters[direction.Opposite()];

        // Patterns must be defined in a clockwise order - check if this is the case if you're having problems
        return Equals(firstAdapter.Pattern, new string(secondAdapter.Pattern.Reverse().ToArray()));
    }

    public bool IsAdapterMandatory(Point point, TileResult neighbourTile)
    {
        var direction = Direction.GetCardinalDirection(point, neighbourTile.Point);

        var firstAdapter = Adapters[direction];

        return MandatoryAdapters.Any(m => m.Pattern == firstAdapter.Pattern);
    }
}