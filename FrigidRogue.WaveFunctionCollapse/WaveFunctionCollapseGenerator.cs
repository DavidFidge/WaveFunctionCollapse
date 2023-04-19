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
    private Dictionary<TileContent, int> _tileLimits = new();

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

        foreach (var tile in tileAttributes.Tiles.Keys)
        {
            var tileContent = new TileContent
            {
                Name = tile,
                Texture = textures[tile],
                Attributes = tileAttributes.Tiles[tile]
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
        _tileLimits.Clear();

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
            else
            {
                _tileChoicesPerPoint.Add(tileResult.Point, _tiles.ToList());
            }
        }

        foreach (var item in _tileContent.Where(t => t.Attributes.Limit >= 0))
        {
            if (item.Attributes.Limit == 0 && !item.Attributes.CanExceedLimitIfOnlyValidTile)
            {
                throw new Exception($"Tile {item.Name} is defined with a Limit of zero and CanExceedLimitIfOnlyValidTile set to false.  This is not allowed as it would mean no tiles could be placed.");
            }

            _tileLimits.Add(item, item.Attributes.Limit);
        }
    }

    public NextStepResult Execute()
    {
        var result = NextStepResult.Continue();

        while (result.IsContinue)
        {
            result = ExecuteNextStep();
        }

        return result;
    }

    public NextStepResult ExecuteNextStep()
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

        if (!_entropy.Any())
            return NextStepResult.Complete();

        return NextStepResult.Continue();
    }

    private List<TileChoice> RemoveTileChoicesWhereLimitsReached(List<TileChoice> possibleTileChoices)
    {
        foreach (var tile in _tileLimits.Keys)
        {
            if (_tileLimits[tile] == 0 && !tile.Attributes.CanExceedLimitIfOnlyValidTile)
            {
                possibleTileChoices = possibleTileChoices.Where(t => t.TileContent != tile).ToList();
            }
        }

        return possibleTileChoices;
    }

    private static TileResult GetNextUncollapsedTile(List<TileResult> entropy)
    {
        var lowestEntropy = entropy
            .TakeWhile(e => e.Entropy == entropy.First().Entropy)
            .ToList();

        var nextUncollapsedTile = lowestEntropy[GlobalRandom.DefaultRNG.RandomIndex(lowestEntropy)];

        return nextUncollapsedTile;
    }

    private TileChoice ChooseTile(List<TileChoice> possibleTileChoices)
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

        if (_tileLimits.ContainsKey(chosenTile.TileContent))
        {
            if (_tileLimits[chosenTile.TileContent] > 0)
                _tileLimits[chosenTile.TileContent]--;
        }

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
                if (_tileLimits.ContainsKey(retryTile.TileChoice.TileContent))
                {
                    _tileLimits[retryTile.TileChoice.TileContent]++;
                }

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
        var validTiles = _tileChoicesPerPoint[chosenTile.Point];

        if (!validTiles.Any())
            return new List<TileChoice>();

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

        validTiles = RemoveTileChoicesWhereLimitsReached(validTiles);

        return validTiles;
    }
}