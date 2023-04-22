using Microsoft.Xna.Framework.Graphics;
using NCalc;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse;

public class TileContent
{
    public string Name { get; set; }
    public TileAttribute Attributes { get; set; }
    public Texture2D Texture { get; set; }
    public List<TileChoice> TileChoices { get; set; }

    public void CreateTiles()
    {
        TileChoices = new List<TileChoice>();
        var adapterStrings = Attributes.Adapters.Split(",").Select(a => a.Trim()).ToList();

        var directions = new List<Direction>
        {
            Direction.Up,
            Direction.Right,
            Direction.Down,
            Direction.Left
        };

        if (Attributes.Symmetry == "X")
        {
            var adapters = directions
                .Select((d, index) => new { Direction = d, Index = index })
                .ToDictionary(d => d.Direction, d => (Adapter)adapterStrings[d.Index]);

            TileChoices.Add(new TileChoice(this, adapters));
        }
        else if (Attributes.Symmetry == "^")
        {
            // Loop through the four rotations
            for (var i = 0; i < 4; i++)
            {
                var adapters = new Dictionary<Direction, Adapter>(4);

                for (var j = 0; j < 4; j++)
                {
                    // Populate adapters for each rotation
                    var direction = directions[j];
                    var index = GoRogue.MathHelpers.WrapAround(j - i, directions.Count);
                    var adapterString = adapterStrings[index];

                    adapters.Add(direction, adapterString);
                }

                var rotation = i switch
                {
                    1 => (float)Math.PI / 2,
                    2 => (float)Math.PI,
                    3 => (float)-Math.PI / 2,
                    _ => 0f
                };

                TileChoices.Add(new TileChoice(this, adapters, rotation: rotation));
            }
        }
        else if (Attributes.Symmetry == "I")
        {
            for (var i = 0; i < 2; i++)
            {
                var adapters = new Dictionary<Direction, Adapter>(4);

                for (var j = 0; j < 4; j++)
                {
                    // Populate adapters for each rotation
                    var direction = directions[j];
                    var index = GoRogue.MathHelpers.WrapAround(j - i, directions.Count);
                    var adapterString = adapterStrings[index];

                    adapters.Add(direction, adapterString);
                }

                var rotation = 0f;

                if (i == 1)
                    rotation = (float)Math.PI / 2f;

                TileChoices.Add(new TileChoice(this, adapters, rotation: rotation));
            }
        }
        else if (Attributes.Symmetry == "/")
        {
            for (var i = 0; i < 2; i++)
            {
                var adapters = new Dictionary<Direction, Adapter>(4);

                for (var j = 0; j < 4; j++)
                {
                    // Populate adapters for each rotation
                    var direction = directions[j];
                    var index = GoRogue.MathHelpers.WrapAround(j - (i * 2), directions.Count);
                    var adapterString = adapterStrings[index];

                    adapters.Add(direction, adapterString);
                }

                var rotation = 0f;

                if (i == 1)
                    rotation = (float)Math.PI;

                TileChoices.Add(new TileChoice(this, adapters, rotation: rotation));
            }
        }

        foreach (var tile in TileChoices.ToList())
        {
            if (Attributes.MirrorHorizontally && Attributes.MirrorVertically)
            {
                var adapters = tile.Adapters.ToDictionary(d => d.Key, d => d.Value.Clone());

                adapters[Direction.Up].Pattern = new String(adapters[Direction.Up].Pattern.Reverse().ToArray());
                adapters[Direction.Down].Pattern = new String(adapters[Direction.Down].Pattern.Reverse().ToArray());
                adapters[Direction.Left].Pattern = new String(adapters[Direction.Left].Pattern.Reverse().ToArray());
                adapters[Direction.Right].Pattern = new String(adapters[Direction.Right].Pattern.Reverse().ToArray());

                TileChoices.Add(new TileChoice(this, adapters, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically));
            }
            if (Attributes.MirrorHorizontally)
            {
                var adapters = tile.Adapters.ToDictionary(d => d.Key, d => d.Value.Clone());

                adapters[Direction.Up].Pattern = new String(adapters[Direction.Up].Pattern.Reverse().ToArray());
                adapters[Direction.Down].Pattern = new String(adapters[Direction.Down].Pattern.Reverse().ToArray());

                TileChoices.Add(new TileChoice(this, adapters, SpriteEffects.FlipHorizontally));
            }
            if (Attributes.MirrorVertically)
            {
                var adapters = tile.Adapters.ToDictionary(d => d.Key, d => d.Value.Clone());

                adapters[Direction.Left].Pattern = new String(adapters[Direction.Left].Pattern.Reverse().ToArray());
                adapters[Direction.Right].Pattern = new String(adapters[Direction.Right].Pattern.Reverse().ToArray());

                TileChoices.Add(new TileChoice(this, adapters, SpriteEffects.FlipVertically));
            }
        }
    }

    public bool IsWithinInitialisationRule(Point point, int mapWidth, int mapHeight)
    {
        if (String.IsNullOrEmpty(Attributes.InitialisationRule))
            return false;

        var expression = new Expression(Attributes.InitialisationRule);

        expression.Parameters["X"] = point.X;
        expression.Parameters["Y"] = point.Y;
        expression.Parameters["MaxX"] = mapWidth - 1;
        expression.Parameters["MaxY"] = mapHeight - 1;

        var isPointWithinEvaluationRule = (bool)expression.Evaluate();

        return isPointWithinEvaluationRule;
    }
}
