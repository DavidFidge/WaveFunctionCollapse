using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse;

public class TileChoice
{
    private TileContent _tileContent;
    public int Weight => _tileContent.Attributes.Weight;
    public Texture2D Texture => _tileContent.Texture;

    public SpriteEffects SpriteEffects;
    public float Rotation; 
    public Dictionary<Direction, Adapter> Adapters { get; set; }

    public TileChoice(TileContent tileContent, Dictionary<Direction, Adapter> adapters, SpriteEffects spriteEffects = SpriteEffects.None, float rotation = 0f)
    {
        _tileContent = tileContent;
        Adapters = adapters;
        SpriteEffects = spriteEffects;
        Rotation = rotation;
    }

    public bool CanAdaptTo(TileChoice tileChoice, Point point, TileResult neighbourTile)
    {
        var direction = Direction.GetCardinalDirection(point, neighbourTile.Point);

        var firstAdapter = tileChoice.Adapters[direction];
        var secondAdapter = neighbourTile.TileChoice.Adapters[direction.Opposite()];

        // Ensure patterns are defined in a clockwise order
        return Equals(firstAdapter.Pattern, secondAdapter.Pattern);
    }
}