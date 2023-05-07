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

        var prohibitedEmptyNeighboursFromConfiguration = Attributes.ProhibitedEmptyNeighbourRules ?? "None";

        var prohibitedEmptyNeighboursStrings = prohibitedEmptyNeighboursFromConfiguration
            .Split(",")
            .Select(a => a.Trim().Replace("|",","))
            .ToList();

        while (prohibitedEmptyNeighboursStrings.Count < 4)
            prohibitedEmptyNeighboursStrings.Add("None");

        var entropyWeights = Attributes.EntropyWeights ?? Attributes.Weight.ToString();

        var entropyWeightsParsed = entropyWeights
            .Split(",")
            .Select(a => int.Parse(a.Trim()))
            .ToList();

        while (entropyWeightsParsed.Count < 4)
            entropyWeightsParsed.Add(Attributes.Weight);

        var canConnectToSelf = Attributes.CanConnectToSelf ?? "true,true,true,true";
        
        var canConnectToSelfParsed = canConnectToSelf
            .Split(",")
            .Select(a => bool.Parse(a.Trim()))
            .ToList();

        while (canConnectToSelfParsed.Count < 4)
            canConnectToSelfParsed.Add(true);
        
        var directions = new List<Direction>
        {
            Direction.Up,
            Direction.Right,
            Direction.Down,
            Direction.Left
        };

        if (Attributes.Symmetry == "X")
        {
            var adapters = ConvertDirectionalConstraints(directions, adapterStrings, v => (Adapter)v);
            var prohibitedEmptyNeighbours = ConvertDirectionalConstraints(directions, prohibitedEmptyNeighboursStrings, v => Enum.Parse<ProhibitedEmptyNeighbourFlags>(v));
            var entropyWeightsDictionary = ConvertDirectionalConstraints(directions, entropyWeightsParsed, v => v);
            var canConnectToSelfDictionary = ConvertDirectionalConstraints(directions, canConnectToSelfParsed, v => v);

            TileChoices.Add(new TileChoice(this, adapters, prohibitedEmptyNeighbours, entropyWeightsDictionary, canConnectToSelfDictionary));
        }
        else if (Attributes.Symmetry == "^")
        {
            // Loop through the four rotations
            for (var i = 0; i < 4; i++)
            {
                var wrapAroundFunction = new Func<int, int, int>((rotationIndex, directionIndex) => GoRogue.MathHelpers.WrapAround(directionIndex - rotationIndex, directions.Count));

                var adapters = RotateDirectionalConstraints(directions, adapterStrings, i, v => (Adapter)v, wrapAroundFunction);
                var prohibitedEmptyNeighbours = RotateDirectionalConstraints(directions, prohibitedEmptyNeighboursStrings, i, v => Enum.Parse<ProhibitedEmptyNeighbourFlags>(v), wrapAroundFunction);
                var entropyWeightsDictionary = RotateDirectionalConstraints(directions, entropyWeightsParsed, i, v => v, wrapAroundFunction);
                var canConnectToSelfDictionary = RotateDirectionalConstraints(directions, canConnectToSelfParsed, i, v => v, wrapAroundFunction);   

                var rotation = i switch
                {
                    1 => (float)Math.PI / 2,
                    2 => (float)Math.PI,
                    3 => (float)-Math.PI / 2,
                    _ => 0f
                };

                TileChoices.Add(new TileChoice(this, adapters, prohibitedEmptyNeighbours, entropyWeightsDictionary, canConnectToSelfDictionary, rotation: rotation));
            }
        }
        else if (Attributes.Symmetry == "I")
        {
            for (var i = 0; i < 2; i++)
            {
                var wrapAroundFunction = new Func<int, int, int>((rotationIndex, directionIndex) => GoRogue.MathHelpers.WrapAround(directionIndex - rotationIndex, directions.Count));

                var adapters = RotateDirectionalConstraints(directions, adapterStrings, i, v => (Adapter)v, wrapAroundFunction);
                var prohibitedEmptyNeighbours = RotateDirectionalConstraints(directions, prohibitedEmptyNeighboursStrings, i, v => Enum.Parse<ProhibitedEmptyNeighbourFlags>(v), wrapAroundFunction);
                var entropyWeightsDictionary = RotateDirectionalConstraints(directions, entropyWeightsParsed, i, v => v, wrapAroundFunction);
                var canConnectToSelfDictionary = RotateDirectionalConstraints(directions, canConnectToSelfParsed, i, v => v, wrapAroundFunction);   

                var rotation = 0f;

                if (i == 1)
                    rotation = (float)Math.PI / 2f;

                TileChoices.Add(new TileChoice(this, adapters, prohibitedEmptyNeighbours, entropyWeightsDictionary, canConnectToSelfDictionary, rotation: rotation));
            }
        }
        else if (Attributes.Symmetry == "/")
        {
            for (var i = 0; i < 2; i++)
            {
                var wrapAroundFunction = new Func<int, int, int>((rotationIndex, directionIndex) => GoRogue.MathHelpers.WrapAround(directionIndex - (rotationIndex * 2), directions.Count));

                var adapters = RotateDirectionalConstraints(directions, adapterStrings, i, v => (Adapter)v, wrapAroundFunction);
                var prohibitedEmptyNeighbours = RotateDirectionalConstraints(directions, prohibitedEmptyNeighboursStrings, i, v => Enum.Parse<ProhibitedEmptyNeighbourFlags>(v), wrapAroundFunction);
                var entropyWeightsDictionary = RotateDirectionalConstraints(directions, entropyWeightsParsed, i, v => v, wrapAroundFunction);
                var canConnectToSelfDictionary = RotateDirectionalConstraints(directions, canConnectToSelfParsed, i, v => v, wrapAroundFunction);   

                var rotation = 0f;

                if (i == 1)
                    rotation = (float)Math.PI;

                TileChoices.Add(new TileChoice(this, adapters, prohibitedEmptyNeighbours, entropyWeightsDictionary, canConnectToSelfDictionary, rotation: rotation));
            }
        }

        foreach (var tile in TileChoices.ToList())
        {
            if (Attributes.FlipHorizontally && Attributes.FlipVertically)
            {
                var adapters = tile.Adapters.ToDictionary(d => d.Key, d => d.Value.Clone());

                (adapters[Direction.Up].Pattern, adapters[Direction.Down].Pattern) = (adapters[Direction.Down].Pattern, adapters[Direction.Up].Pattern);
                (adapters[Direction.Left].Pattern, adapters[Direction.Right].Pattern) = (adapters[Direction.Right].Pattern, adapters[Direction.Left].Pattern);

                var prohibitedEmptyNeighbours = tile.ProhibitedEmptyNeighbours.ToDictionary(d => d.Key, d => d.Value);

                (prohibitedEmptyNeighbours[Direction.Up], prohibitedEmptyNeighbours[Direction.Down]) = (prohibitedEmptyNeighbours[Direction.Down], prohibitedEmptyNeighbours[Direction.Up]);
                (prohibitedEmptyNeighbours[Direction.Left], prohibitedEmptyNeighbours[Direction.Right]) = (prohibitedEmptyNeighbours[Direction.Right], prohibitedEmptyNeighbours[Direction.Left]);

                var entropyWeightDictionary = tile.EntropyWeights.ToDictionary(d => d.Key, d => d.Value);

                (entropyWeightDictionary[Direction.Up], entropyWeightDictionary[Direction.Down]) = (entropyWeightDictionary[Direction.Down], entropyWeightDictionary[Direction.Up]);
                (entropyWeightDictionary[Direction.Left], entropyWeightDictionary[Direction.Right]) = (entropyWeightDictionary[Direction.Right], entropyWeightDictionary[Direction.Left]);

                var canConnectToSelfDictionary = tile.CanConnectToSelf.ToDictionary(d => d.Key, d => d.Value);

                (canConnectToSelfDictionary[Direction.Up], canConnectToSelfDictionary[Direction.Down]) = (canConnectToSelfDictionary[Direction.Down], canConnectToSelfDictionary[Direction.Up]);
                (canConnectToSelfDictionary[Direction.Left], canConnectToSelfDictionary[Direction.Right]) = (canConnectToSelfDictionary[Direction.Right], canConnectToSelfDictionary[Direction.Left]);

                TileChoices.Add(new TileChoice(this, adapters, prohibitedEmptyNeighbours, entropyWeightDictionary, canConnectToSelfDictionary, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically));
            }
            if (Attributes.FlipHorizontally)
            {
                var adapters = tile.Adapters.ToDictionary(d => d.Key, d => d.Value.Clone());

                adapters[Direction.Up].Pattern = new String(adapters[Direction.Up].Pattern.Reverse().ToArray());
                adapters[Direction.Down].Pattern = new String(adapters[Direction.Down].Pattern.Reverse().ToArray());

                var oldLeft = adapters[Direction.Left].Pattern;
                adapters[Direction.Left].Pattern = new String(adapters[Direction.Right].Pattern.Reverse().ToArray());
                adapters[Direction.Right].Pattern = new String(oldLeft.Reverse().ToArray());

                var prohibitedEmptyNeighbours = tile.ProhibitedEmptyNeighbours.ToDictionary(d => d.Key, d => d.Value);
                (prohibitedEmptyNeighbours[Direction.Left], prohibitedEmptyNeighbours[Direction.Right]) = (prohibitedEmptyNeighbours[Direction.Right], prohibitedEmptyNeighbours[Direction.Left]);

                var entropyWeightDictionary = tile.EntropyWeights.ToDictionary(d => d.Key, d => d.Value);
                (entropyWeightDictionary[Direction.Left], entropyWeightDictionary[Direction.Right]) = (entropyWeightDictionary[Direction.Right], entropyWeightDictionary[Direction.Left]);

                var canConnectToSelfDictionary = tile.CanConnectToSelf.ToDictionary(d => d.Key, d => d.Value);
                (canConnectToSelfDictionary[Direction.Left], canConnectToSelfDictionary[Direction.Right]) = (canConnectToSelfDictionary[Direction.Right], canConnectToSelfDictionary[Direction.Left]);

                TileChoices.Add(new TileChoice(this, adapters, prohibitedEmptyNeighbours, entropyWeightDictionary, canConnectToSelfDictionary, SpriteEffects.FlipHorizontally));
            }
            if (Attributes.FlipVertically)
            {
                var adapters = tile.Adapters.ToDictionary(d => d.Key, d => d.Value.Clone());

                var oldUp = adapters[Direction.Up].Pattern;
                adapters[Direction.Up].Pattern = new String(adapters[Direction.Down].Pattern.Reverse().ToArray());
                adapters[Direction.Down].Pattern = new String(oldUp.Reverse().ToArray());

                adapters[Direction.Left].Pattern = new String(adapters[Direction.Left].Pattern.Reverse().ToArray());
                adapters[Direction.Right].Pattern = new String(adapters[Direction.Right].Pattern.Reverse().ToArray());

                var prohibitedEmptyNeighbours = tile.ProhibitedEmptyNeighbours.ToDictionary(d => d.Key, d => d.Value);
                (prohibitedEmptyNeighbours[Direction.Up], prohibitedEmptyNeighbours[Direction.Down]) = (prohibitedEmptyNeighbours[Direction.Down], prohibitedEmptyNeighbours[Direction.Up]);

                var entropyWeightDictionary = tile.EntropyWeights.ToDictionary(d => d.Key, d => d.Value);
                (entropyWeightDictionary[Direction.Up], entropyWeightDictionary[Direction.Down]) = (entropyWeightDictionary[Direction.Down], entropyWeightDictionary[Direction.Up]);
                
                var canConnectToSelfDictionary = tile.CanConnectToSelf.ToDictionary(d => d.Key, d => d.Value);
                (canConnectToSelfDictionary[Direction.Up], canConnectToSelfDictionary[Direction.Down]) = (canConnectToSelfDictionary[Direction.Down], canConnectToSelfDictionary[Direction.Up]);
                
                TileChoices.Add(new TileChoice(this, adapters, prohibitedEmptyNeighbours, entropyWeightDictionary, canConnectToSelfDictionary, SpriteEffects.FlipVertically));
            }
        }
    }

    private Dictionary<Direction, TResult> ConvertDirectionalConstraints<TInput, TResult>(IList<Direction> directions, IList<TInput> directionalConstraints, Func<TInput, TResult> valueConverter)
    {
       return RotateDirectionalConstraints(directions, directionalConstraints, 0, valueConverter, (rotationIndex, j) => j);
    }

    private Dictionary<Direction, TResult> RotateDirectionalConstraints<TInput, TResult>(IList<Direction> directions,
        IList<TInput> directionalConstraints, int rotationIndex, Func<TInput, TResult> valueConverter, Func<int, int, int> wrapAroundFunction)
    {
        var result = new Dictionary<Direction, TResult>(directions.Count);

        for (var j = 0; j < directions.Count; j++)
        {
            var direction = directions[j];
            var index = GoRogue.MathHelpers.WrapAround(wrapAroundFunction(rotationIndex, j), directions.Count);
            var value = valueConverter(directionalConstraints[index]);
            result.Add(direction, value);
        }

        return result;
    }
}
