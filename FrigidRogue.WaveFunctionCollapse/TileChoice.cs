using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse;

public class TileChoice
{
    private TileContent _tileContent;

    public TileContent TileContent => _tileContent;
    public int Weight => _tileContent.Attributes.Weight;
    public Texture2D Texture => _tileContent.Texture;

    public SpriteEffects SpriteEffects;
    public float Rotation;
    public Dictionary<Direction, Adapter> Adapters { get; set; }
    public List<Adapter> MandatoryAdapters { get; set; }

    public TileChoice(TileContent tileContent, Dictionary<Direction, Adapter> adapters, SpriteEffects spriteEffects = SpriteEffects.None, float rotation = 0f)
    {
        _tileContent = tileContent;
        Adapters = adapters;
        SpriteEffects = spriteEffects;
        Rotation = rotation;

        if (string.IsNullOrEmpty(tileContent.Attributes.MandatoryAdapters))
            MandatoryAdapters = new List<Adapter>();
        else
        {
            MandatoryAdapters = tileContent.Attributes.MandatoryAdapters
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

        // Ensure patterns are defined in a clockwise order
        return Equals(firstAdapter.Pattern, new string(secondAdapter.Pattern.Reverse().ToArray()));
    }

    public bool IsAdapterMandatory(Point point, TileResult neighbourTile)
    {
        // This method assumes that CanAdaptTo has already succeeded!
        var direction = Direction.GetCardinalDirection(point, neighbourTile.Point);

        var firstAdapter = Adapters[direction];

        return MandatoryAdapters.Any(m => m.Pattern == firstAdapter.Pattern);
    }
}