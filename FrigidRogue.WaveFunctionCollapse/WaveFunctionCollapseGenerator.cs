using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.Random;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using SadRogue.Primitives;
using ShaiRandom.Generators;

using Point = SadRogue.Primitives.Point;

namespace FrigidRogue.WaveFunctionCollapse;

public class WaveFunctionCollapseGenerator
{
    private List<TileChoice> _tiles = new();
    public TileResult[] CurrentState { get; private set; }
    public List<TileChoice> Tiles => _tiles;

    private Dictionary<Point, List<TileChoice>> _tileChoicesPerPoint = new();
    private List<TileContent> _tileContent = new();
    private WaveFunctionCollapseGeneratorOptions _options;
    private List<TileResult> _entropy = new();

    public void CreateTiles(ContentManager contentManager, string contentPath, List<string> tileNames)
    {
        var tileAttributes = contentManager.Load<TileAttributes>($"{contentPath}/TileAttributes.json",
            new JsonContentLoader());

        var textures = tileNames
            .ToDictionary(
                a => a.Replace(".png", ""),
                a => contentManager.Load<Texture2D>($"{contentPath}/{a}"));

        CreateTiles(textures, tileAttributes);
    }

    public void CreateTiles(ContentManager contentManager, string contentPath)
    {
        // Assumes you have a resource in your content called "Content" which is a list of the items in your Content.mgcb file.  Refer to
        // "Content.npl" item at https://github.com/Martenfur/NoPipeline for an idea on how to do this if you want to use this constructor.
        var assetsList = contentManager.Load<string[]>("Content");
        var tileAttributes = contentManager.Load<TileAttributes>($"{contentPath}/TileAttributes.json",
            new JsonContentLoader());

        var textures = assetsList
            .Where(a => a.StartsWith(contentPath) && !a.EndsWith(".json"))
            .ToDictionary(
                a => a.Split("/").Last().Replace(".png", ""),
                a => contentManager.Load<Texture2D>(a));

        CreateTiles(textures, tileAttributes);
    }

    public void CreateTiles(Dictionary<string, Texture2D> textures, TileAttributes tileAttributes)
    {
        _tileContent.Clear();

        foreach (var texture in textures)
        {
            var tileContent = new TileContent
            {
                Name = texture.Key,
                Texture = texture.Value,
                Attributes = tileAttributes.Tiles[texture.Key]
            };

            _tileContent.Add(tileContent);

            tileContent.CreateTiles();
        }

        _tiles.Clear();
        _tiles.AddRange(_tileContent.SelectMany(t => t.TileChoices));
    }

    public void Reset(WaveFunctionCollapseGeneratorOptions options)
    {
        _options = options.Clone();
        _tileChoicesPerPoint.Clear();

        CurrentState = new TileResult[options.MapWidth * options.MapHeight];
        
        for (var y = 0; y < options.MapHeight; y++)
        {
            for (var x = 0; x < options.MapWidth; x++)
            {
                var point = new Point(x, y);

                var tileResult = new TileResult(point, options.MapWidth);
                CurrentState[point.ToIndex(options.MapWidth)] = tileResult;
                _entropy.Add(tileResult);
            }
        }

        foreach (var tileResult in CurrentState)
        {
            tileResult.SetNeighbours(CurrentState, options.MapWidth, options.MapHeight);

            var initialisationSelection = _tileContent
                .Where(t => t.IsWithinInitialisationRule(tileResult.Point, options.MapWidth, options.MapHeight))
                .SelectMany(t => t.TileChoices)
                .ToList();

            if (initialisationSelection.Any())
            {
                // By reducing entropy the tile will be among the first to be processed and thus initialised with
                // the tile choice as given by the initialisation rule
                // Reducing by 5 as tiles can normally be reduced by 4 (4 neighbours), so reducing by 5 ensure it is always
                // processed before non-initialised tiles.
                tileResult.Entropy -= 5;

                _tileChoicesPerPoint.Add(tileResult.Point, initialisationSelection);
            }
        }
    }

    public NextStepResult NextStep()
    {
        if (!_entropy.Any())
            return NextStepResult.Complete(); 
        
        _entropy.Sort((a, b) => a.Entropy - b.Entropy );

        var nextUncollapsedTile = GetNextUncollapsedTile(_entropy);

        var possibleTileChoices = GetPossibleTileChoices(nextUncollapsedTile);

        if (!possibleTileChoices.Any())
        {
            if (_options.FallbackAttempts == 0)
                return NextStepResult.Failed();

            RevertTilesInRadius(nextUncollapsedTile, _entropy.First().Entropy);

            return NextStepResult.Continue();
        }

        var chosenTile = ChooseTile(possibleTileChoices);

        nextUncollapsedTile.SetTile(chosenTile);
        _entropy.Remove(nextUncollapsedTile);

        return NextStepResult.Continue();
    }

    private static TileResult GetNextUncollapsedTile(List<TileResult> entropy)
    {
        var lowestEntropy = entropy
            .TakeWhile(e => e.Entropy == entropy.First().Entropy)
            .ToList();

        var nextUncollapsedTile = lowestEntropy[GlobalRandom.DefaultRNG.RandomIndex(lowestEntropy)];

        return nextUncollapsedTile;
    }

    private static TileChoice ChooseTile(List<TileChoice> possibleTileChoices)
    {
        var sumWeights = possibleTileChoices.Sum(t => t.Weight);

        var randomNumber = GlobalRandom.DefaultRNG.NextInt(0, sumWeights);

        var i = 0;
        while (randomNumber > 0)
        {
            randomNumber -= possibleTileChoices[i].Weight;

            if (randomNumber < 0)
                break;

            i++;
        }

        var chosenTile = possibleTileChoices[i];
        return chosenTile;
    }

    private void RevertTilesInRadius(TileResult chosenTile, int currentLowestEntropy)
    {
        var retryPoints = chosenTile.Point
            .NeighboursOutwardsFrom(_options.FallbackRadius, 0, _options.MapWidth - 1, 0, _options.MapHeight - 1)
            .OrderBy(p => (int)Distance.Manhattan.Calculate(p, chosenTile.Point))
            .ToList();

        foreach (var retryPoint in retryPoints)
        {
            var retryTile = CurrentState[retryPoint.ToIndex(_options.MapWidth)];

            if (retryTile.IsCollapsed)
            {
                retryTile.TileChoice = null;

                // Set entropy to tiles being retried to lowest so that they get processed first.  The further out the tile
                // the earlier it should be reprocessed.
                retryTile.Entropy = currentLowestEntropy - (int)Distance.Manhattan.Calculate(retryPoint, chosenTile.Point);
                _entropy.Add(retryTile);
            }
        }

        _options.FallbackAttempts--;
        _options.FallbackRadius += _options.FallbackRadiusIncrement;
    }

    private List<TileChoice> GetPossibleTileChoices(TileResult chosenTile)
    {
        var validTiles = _tiles.ToList();

        if (_tileChoicesPerPoint.TryGetValue(chosenTile.Point, out var value))
            validTiles = value;

        foreach (var neighbour in chosenTile.Neighbours)
        {
            if (!neighbour.IsCollapsed)
            {
                neighbour.Entropy--;
            }
            else
            {
                validTiles = validTiles.Where(t => t.CanAdaptTo(t, chosenTile.Point, neighbour)).ToList();
            }
        }

        return validTiles;
    }
}