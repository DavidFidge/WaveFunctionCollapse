using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.Xna.Framework.Graphics;
using NCalc;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse;

public class TileChoice
{
    public TileTemplate TileTemplate { get; }
    public SpriteEffects SpriteEffects { get; }
    public float Rotation { get; }
    public Dictionary<Direction, Adapter> Adapters { get; }
    public Dictionary<Direction, ProhibitedEmptyNeighbourFlags> ProhibitedEmptyNeighbours { get; }
    public Dictionary<Direction, int> EntropyWeights { get; }
    public Adapter MandatoryAdapter { get; }
    public int Weight => TileTemplate.Attributes.Weight;
    public Texture2D Texture => TileTemplate.Texture;

    public TileChoice(TileTemplate tileTemplate, Dictionary<Direction, Adapter> adapters, Dictionary<Direction, ProhibitedEmptyNeighbourFlags> prohibitedEmptyNeighbours, Dictionary<Direction, int> entropyWeights, SpriteEffects spriteEffects = SpriteEffects.None, float rotation = 0f)
    {
        TileTemplate = tileTemplate;
        Adapters = adapters;
        SpriteEffects = spriteEffects;
        Rotation = rotation;
        EntropyWeights = entropyWeights;

        MandatoryAdapter = tileTemplate.Attributes.MandatoryAdapters;

        if (prohibitedEmptyNeighbours == null)
        {
            prohibitedEmptyNeighbours = new Dictionary<Direction, ProhibitedEmptyNeighbourFlags>
            {
                { Direction.Up, ProhibitedEmptyNeighbourFlags.None }, { Direction.Right, ProhibitedEmptyNeighbourFlags.None }, { Direction.Down, ProhibitedEmptyNeighbourFlags.None }, { Direction.Left, ProhibitedEmptyNeighbourFlags.None }
            };
        }

        ProhibitedEmptyNeighbours = prohibitedEmptyNeighbours;

        if (entropyWeights == null)
        {
            entropyWeights = new Dictionary<Direction, int>
            {
                { Direction.Up, Weight }, { Direction.Right, Weight }, { Direction.Down, Weight }, { Direction.Left, Weight }
            };
        }

        EntropyWeights = entropyWeights;
    }

    public bool CanConnectToCategory(Point point, TileResult neighbourTile)
    {
        if (TileTemplate.Attributes.CanConnectToCategories == null)
            return true;

        if (String.IsNullOrEmpty(neighbourTile.ChosenTile.TileTemplate.Attributes.Category))
            return true;

        var canConnect =
            TileTemplate.Attributes.CanConnectToCategories.Any(c =>
                c == neighbourTile.ChosenTile.TileTemplate.Attributes.Category);

        return canConnect;
    }

    public bool CanAdaptTo(Point point, TileResult neighbourTile)
    {
        var direction = Direction.GetCardinalDirection(point, neighbourTile.Point);

        var firstAdapter = Adapters[direction];
        var secondAdapter = neighbourTile.ChosenTile.Adapters[direction.Opposite()];

        return firstAdapter.Matches(secondAdapter);
    }

    public bool IsAdapterMandatory(Point point, TileResult neighbourTile)
    {
        var direction = Direction.GetCardinalDirection(point, neighbourTile.Point);

        var firstAdapter = neighbourTile.ChosenTile.Adapters[direction.Opposite()];

        return MandatoryAdapter.MatchesMandatory(firstAdapter);
    }

    public bool PassesPlacementRule(Point point, int mapWidth, int mapHeight)
    {
        if (String.IsNullOrEmpty(TileTemplate.Attributes.PlacementRule))
            return true;

        var expression = new Expression(TileTemplate.Attributes.PlacementRule);

        expression.Parameters["X"] = point.X;
        expression.Parameters["Y"] = point.Y;
        expression.Parameters["MaxX"] = mapWidth - 1;
        expression.Parameters["MaxY"] = mapHeight - 1;

        var isPointWithinEvaluationRule = (bool)expression.Evaluate();

        return isPointWithinEvaluationRule;
    }
}