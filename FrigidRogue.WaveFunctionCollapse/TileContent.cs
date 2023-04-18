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
            for (var i = 0; i < 4; i++)
            {
                var adapters = directions
                    .Select((d, index) => new { Direction = d, Index = index })
                    .ToDictionary(d => d.Direction, d => (Adapter)adapterStrings[GoRogue.MathHelpers.WrapAround(d.Index - i,directions.Count)]);

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
            var adapters = directions
                .Select((d, i) => new { Direction = d, Index = i})
                .ToDictionary(d => d.Direction, d => (Adapter)adapterStrings[d.Index]);

            TileChoices.Add(new TileChoice(this, adapters));

            adapters = directions
                .Select((d, i) => new { Direction = d, Index = i})
                .ToDictionary(d => d.Direction, d => (Adapter)adapterStrings[GoRogue.MathHelpers.WrapAround(d.Index - 1,directions.Count)]);

            TileChoices.Add(new TileChoice(this, adapters, rotation: (float)Math.PI / 2));
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