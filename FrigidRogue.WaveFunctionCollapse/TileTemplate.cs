using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace FrigidRogue.WaveFunctionCollapse;

public class TileTemplate
{
    public string Name { get; }
    public TileAttribute Attributes { get; }
    public Texture2D Texture { get; }
    public List<TileChoice> TileChoices { get; }

    public TileTemplate(string name, TileAttribute attributes, Texture2D texture)
    {
        Name = name;
        Attributes = attributes;
        Texture = texture;
        TileChoices = new List<TileChoice>();
    }

    public void CreateTiles()
    {
        TileChoices.Clear();

        var adaptersFromConfiguration = Attributes.Adapters ?? String.Empty;

        var adapterStrings = adaptersFromConfiguration
            .Split(",")
            .Select(a => a.Trim())
            .ToList();

        while (adapterStrings.Count < 4)
            adapterStrings.Add(String.Empty);

        var emptyPlacementFromConfiguration = Attributes.EmptyPlacementRules ?? "true";

        var emptyPlacementStrings = emptyPlacementFromConfiguration
            .Split(",")
            .Select(a => a.Trim())
            .ToList();

        while (emptyPlacementStrings.Count < 4)
            emptyPlacementStrings.Add("true");

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

            var emptyPlacements = directions
                .Select((d, index) => new { Direction = d, Index = index })
                .ToDictionary(d => d.Direction, d => bool.Parse(emptyPlacementStrings[d.Index]));

            TileChoices.Add(new TileChoice(this, adapters, emptyPlacements));
        }
        else if (Attributes.Symmetry == "^")
        {
            // Loop through the four rotations
            for (var i = 0; i < 4; i++)
            {
                var adapters = new Dictionary<Direction, Adapter>(4);
                var emptyPlacements = new Dictionary<Direction, bool>(4);

                for (var j = 0; j < 4; j++)
                {
                    // Populate adapters for each rotation
                    var direction = directions[j];
                    var index = GoRogue.MathHelpers.WrapAround(j - i, directions.Count);

                    var adapterString = adapterStrings[index];
                    adapters.Add(direction, adapterString);

                    var emptyPlacementString = emptyPlacementStrings[index];
                    emptyPlacements.Add(direction, bool.Parse(emptyPlacementString));
                }

                var rotation = i switch
                {
                    1 => (float)Math.PI / 2,
                    2 => (float)Math.PI,
                    3 => (float)-Math.PI / 2,
                    _ => 0f
                };

                TileChoices.Add(new TileChoice(this, adapters, emptyPlacements, rotation: rotation));
            }
        }
        else if (Attributes.Symmetry == "I")
        {
            for (var i = 0; i < 2; i++)
            {
                var adapters = new Dictionary<Direction, Adapter>(4);
                var emptyPlacements = new Dictionary<Direction, bool>(4);

                for (var j = 0; j < 4; j++)
                {
                    // Populate adapters for each rotation
                    var direction = directions[j];
                    var index = GoRogue.MathHelpers.WrapAround(j - i, directions.Count);

                    var adapterString = adapterStrings[index];
                    adapters.Add(direction, adapterString);

                    var emptyPlacementString = emptyPlacementStrings[index];
                    emptyPlacements.Add(direction, bool.Parse(emptyPlacementString));
                }

                var rotation = 0f;

                if (i == 1)
                    rotation = (float)Math.PI / 2f;

                TileChoices.Add(new TileChoice(this, adapters, emptyPlacements, rotation: rotation));
            }
        }
        else if (Attributes.Symmetry == "/")
        {
            for (var i = 0; i < 2; i++)
            {
                var adapters = new Dictionary<Direction, Adapter>(4);
                var emptyPlacements = new Dictionary<Direction, bool>(4);

                for (var j = 0; j < 4; j++)
                {
                    // Populate adapters for each rotation
                    var direction = directions[j];
                    var index = GoRogue.MathHelpers.WrapAround(j - (i * 2), directions.Count);
                    
                    var adapterString = adapterStrings[index];
                    adapters.Add(direction, adapterString);

                    var emptyPlacementString = emptyPlacementStrings[index];
                    emptyPlacements.Add(direction, bool.Parse(emptyPlacementString));
                }

                var rotation = 0f;

                if (i == 1)
                    rotation = (float)Math.PI;

                TileChoices.Add(new TileChoice(this, adapters, emptyPlacements, rotation: rotation));
            }
        }

        foreach (var tile in TileChoices.ToList())
        {
            if (Attributes.FlipHorizontally && Attributes.FlipVertically)
            {
                var adapters = tile.Adapters.ToDictionary(d => d.Key, d => d.Value.Clone());

                (adapters[Direction.Up].Pattern, adapters[Direction.Down].Pattern) = (adapters[Direction.Down].Pattern, adapters[Direction.Up].Pattern);
                (adapters[Direction.Left].Pattern, adapters[Direction.Right].Pattern) = (adapters[Direction.Right].Pattern, adapters[Direction.Left].Pattern);

                var emptyPlacements = tile.EmptyPlacementRules.ToDictionary(d => d.Key, d => d.Value);

                (emptyPlacements[Direction.Up], emptyPlacements[Direction.Down]) = (emptyPlacements[Direction.Down], emptyPlacements[Direction.Up]);
                (emptyPlacements[Direction.Left], emptyPlacements[Direction.Right]) = (emptyPlacements[Direction.Right], emptyPlacements[Direction.Left]);

                TileChoices.Add(new TileChoice(this, adapters, emptyPlacements, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically));
            }
            if (Attributes.FlipHorizontally)
            {
                var adapters = tile.Adapters.ToDictionary(d => d.Key, d => d.Value.Clone());

                adapters[Direction.Up].Pattern = new String(adapters[Direction.Up].Pattern.Reverse().ToArray());
                adapters[Direction.Down].Pattern = new String(adapters[Direction.Down].Pattern.Reverse().ToArray());

                var oldLeft = adapters[Direction.Left].Pattern;
                adapters[Direction.Left].Pattern = new String(adapters[Direction.Right].Pattern.Reverse().ToArray());
                adapters[Direction.Right].Pattern = new String(oldLeft.Reverse().ToArray());

                var emptyPlacements = tile.EmptyPlacementRules.ToDictionary(d => d.Key, d => d.Value);
                (emptyPlacements[Direction.Left], emptyPlacements[Direction.Right]) = (emptyPlacements[Direction.Right], emptyPlacements[Direction.Left]);

                TileChoices.Add(new TileChoice(this, adapters, emptyPlacements, SpriteEffects.FlipHorizontally));
            }
            if (Attributes.FlipVertically)
            {
                var adapters = tile.Adapters.ToDictionary(d => d.Key, d => d.Value.Clone());

                var oldUp = adapters[Direction.Up].Pattern;
                adapters[Direction.Up].Pattern = new String(adapters[Direction.Down].Pattern.Reverse().ToArray());
                adapters[Direction.Down].Pattern = new String(oldUp.Reverse().ToArray());

                adapters[Direction.Left].Pattern = new String(adapters[Direction.Left].Pattern.Reverse().ToArray());
                adapters[Direction.Right].Pattern = new String(adapters[Direction.Right].Pattern.Reverse().ToArray());

                var emptyPlacements = tile.EmptyPlacementRules.ToDictionary(d => d.Key, d => d.Value);
                (emptyPlacements[Direction.Up], emptyPlacements[Direction.Down]) = (emptyPlacements[Direction.Down], emptyPlacements[Direction.Up]);

                TileChoices.Add(new TileChoice(this, adapters, emptyPlacements, SpriteEffects.FlipVertically));
            }
        }
    }
}
