using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse;

public class TileContent
{
    public string Name { get; set; }
    public TileAttribute Attributes { get; set; }
    public Texture2D Texture { get; set; }

    public List<TileChoice> CreateTiles()
    {
        var tiles = new List<TileChoice>();
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

            tiles.Add(new TileChoice(this, adapters));
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

                tiles.Add(new TileChoice(this, adapters, rotation: rotation));
            }
        }
        else if (Attributes.Symmetry == "I")
        {
            var adapters = directions
                .Select((d, i) => new { Direction = d, Index = i})
                .ToDictionary(d => d.Direction, d => (Adapter)adapterStrings[d.Index]);

            tiles.Add(new TileChoice(this, adapters));

            adapters = directions
                .Select((d, i) => new { Direction = d, Index = i})
                .ToDictionary(d => d.Direction, d => (Adapter)adapterStrings[GoRogue.MathHelpers.WrapAround(d.Index - 1,directions.Count)]);

            tiles.Add(new TileChoice(this, adapters, rotation: (float)Math.PI / 2));
        }

        return tiles;
    }
}