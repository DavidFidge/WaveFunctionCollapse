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
    private List<TileResult> _uncollapsedTilesSortedByEntropy = new();
    private Dictionary<TileContent, int> _tileLimits = new();
    private TileAttributes _tileAttributes;
    public int MapWidth => _tileAttributes.Options.MapWidth;
    public int MapHeight => _tileAttributes.Options.MapHeight;
    public int TileWidth => _tiles[0].Texture.Bounds.Width;
    public int TileHeight => _tiles[0].Texture.Bounds.Height;
    
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
            .Where(a => a.StartsWith($"{contentPath}/") && !a.EndsWith(".json"))
            .ToDictionary(
                a => a.Split("/").Last().Replace(".png", ""),
                a => contentManager.Load<Texture2D>(a));

        CreateTiles(textures, tileAttributes);
    }

    public void CreateTiles(Dictionary<string, Texture2D> textures, TileAttributes tileAttributes)
    {
        _tileAttributes = tileAttributes;
        _options = tileAttributes.Options.Clone();
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

    public void Reset()
    {
        _options = _tileAttributes.Options.Clone();
        _uncollapsedTilesSortedByEntropy.Clear();
        _tileChoicesPerPoint.Clear();
        _tileLimits.Clear();

        CurrentState = new TileResult[_options.MapWidth * _options.MapHeight];
        
        for (var y = 0; y < _options.MapHeight; y++)
        {
            for (var x = 0; x < _options.MapWidth; x++)
            {
                var point = new Point(x, y);

                var tileResult = new TileResult(point, _options.MapWidth);
                CurrentState[point.ToIndex(_options.MapWidth)] = tileResult;
                _uncollapsedTilesSortedByEntropy.Add(tileResult);
            }
        }

        foreach (var tileResult in CurrentState)
        {
            tileResult.SetNeighbours(CurrentState, _options.MapWidth, _options.MapHeight);

            var initialisationSelection = _tileContent
                .Where(t => t.IsWithinInitialisationRule(tileResult.Point, _options.MapWidth, _options.MapHeight))
                .SelectMany(t => t.TileChoices)
                .ToList();

            if (initialisationSelection.Any())
            {
                // By reducing entropy the tile will be among the first to be processed and thus initialised with
                // the tile choice as given by the initialisation rule
                tileResult.Entropy -= Int32.MaxValue / 2;
                tileResult.StartingEntropy = tileResult.Entropy;

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
        if (!_uncollapsedTilesSortedByEntropy.Any())
            return NextStepResult.Complete();

        _uncollapsedTilesSortedByEntropy.Sort((a, b) => a.Entropy - b.Entropy );

        var nextUncollapsedTile = GetNextUncollapsedTile(_uncollapsedTilesSortedByEntropy);

        var possibleTileChoices = GetPossibleTileChoices(nextUncollapsedTile);

        if (!possibleTileChoices.Any())
        {
            if (_options.FallbackAttempts == 0)
                return NextStepResult.Failed();

            RevertTilesInRadius(nextUncollapsedTile);

            return NextStepResult.Continue();
        }

        var chosenTile = ChooseTile(possibleTileChoices);

        nextUncollapsedTile.SetTile(chosenTile);
        _uncollapsedTilesSortedByEntropy.Remove(nextUncollapsedTile);

        if (!_uncollapsedTilesSortedByEntropy.Any())
            return NextStepResult.Complete();

        ReduceEntropy(nextUncollapsedTile);

        return NextStepResult.Continue();
    }

    private void ReduceEntropy(TileResult chosenTile)
    {
        foreach (var neighbour in chosenTile.Neighbours.Where(t => !t.IsCollapsed))
        {
            RecalculateEntropy(neighbour);
        }
    }

    private void RecalculateEntropy(TileResult tileToRecalculate)
    {
        tileToRecalculate.Entropy = tileToRecalculate.StartingEntropy;
        var collapsedNeighbours = tileToRecalculate.Neighbours.Where(t => t.IsCollapsed).ToList();

        if (!collapsedNeighbours.Any())
            return;

        switch (_options.EntropyHeuristic)
        {
            case EntropyHeuristic.ReduceByCountOfNeighbours:
                tileToRecalculate.Entropy -= collapsedNeighbours.Count;
                break;
            case EntropyHeuristic.ReduceByWeightOfNeighbours:
                tileToRecalculate.Entropy -= collapsedNeighbours.Sum(t => t.TileChoice.Weight);
                break;
            case EntropyHeuristic.ReduceByCountAndMaxWeightOfNeighbours:
                tileToRecalculate.Entropy = tileToRecalculate.Entropy - collapsedNeighbours.Max(t => t.TileChoice.Weight) - collapsedNeighbours.Count;
                break;
            case EntropyHeuristic.ReduceByCountOfAllTilesMinusPossibleTiles:
                var tileChoices = GetPossibleTileChoices(tileToRecalculate);
                tileToRecalculate.Entropy -= (_tiles.Count - tileChoices.Count);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private List<TileChoice> RemoveTileChoicesWhereLimitsReached(List<TileChoice> possibleTileChoices)
    {
        var possibleReAdds = new List<TileChoice>();

        foreach (var tile in _tileLimits.Keys)
        {
            if (_tileLimits[tile] == 0)
            {
                if (tile.Attributes.CanExceedLimitIfOnlyValidTile)
                {
                    possibleReAdds.AddRange(possibleTileChoices.Where(t => t.TileContent == tile));
                }

                possibleTileChoices = possibleTileChoices.Where(t => t.TileContent != tile).ToList();
            }
        }

        if (possibleTileChoices.Any())
            return possibleTileChoices;

        return possibleReAdds;
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

    private void RevertTilesInRadius(TileResult chosenTile)
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

                _uncollapsedTilesSortedByEntropy.Add(retryTile);
            }
        }

        foreach (var retryPoint in retryPoints)
        {
            var retryTile = CurrentState[retryPoint.ToIndex(_options.MapWidth)];

            RecalculateEntropy(retryTile);
        }

        RecalculateEntropy(chosenTile);

        _options.FallbackAttempts--;
        _options.FallbackRadius += _options.FallbackRadiusIncrement;
    }

    private List<TileChoice> GetPossibleTileChoices(TileResult chosenTile)
    {
        var validTiles = _tileChoicesPerPoint[chosenTile.Point];

        if (!validTiles.Any())
            return new List<TileChoice>();

        foreach (var neighbour in chosenTile.Neighbours.Where(t => t.IsCollapsed))
        {
            validTiles = validTiles.Where(t => t.CanAdaptTo(chosenTile.Point, neighbour)).ToList();
        }

        validTiles = RemoveTileChoicesWhereLimitsReached(validTiles);

        var tilesContainingMandatoryAdapters = validTiles.Where(t => t.MandatoryAdapters.Any()).ToList();

        if (tilesContainingMandatoryAdapters.Any())
        {
            var acceptedTilesContainingMandatoryAdapters = new List<TileChoice>(tilesContainingMandatoryAdapters.Count);

            foreach (var tile in tilesContainingMandatoryAdapters)
            {
                // Look through the neighbours of the chosen tile and remove any that don't have a mandatory adapter.
                foreach (var neighbour in chosenTile.Neighbours.Where(n => n.IsCollapsed))
                {
                    if (tile.IsAdapterMandatory(chosenTile.Point, neighbour))
                    {
                        acceptedTilesContainingMandatoryAdapters.Add(tile);
                        break;
                    }
                }
            }
            
            var tilesToRemove = tilesContainingMandatoryAdapters.Except(acceptedTilesContainingMandatoryAdapters).ToList();

            tilesToRemove.ForEach(t => validTiles.Remove(t));
        }

        return validTiles;
    }
}