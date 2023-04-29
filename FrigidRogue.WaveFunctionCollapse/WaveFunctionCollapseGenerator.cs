using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.WaveFunctionCollapse.Constraints;
using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using ShaiRandom.Generators;

using Point = SadRogue.Primitives.Point;

namespace FrigidRogue.WaveFunctionCollapse;

public class WaveFunctionCollapseGenerator
{
    public TileResult[] Tiles { get; private set; }
    public List<TileChoice> TileChoices { get; }
    public int MapWidth => _mapOptions.MapWidth;
    public int MapHeight => _mapOptions.MapHeight;

    private Dictionary<Point, List<TileChoice>> _tileChoicesPerPoint = new();
    private List<TileTemplate> _tileTemplates = new();
    private GeneratorOptions _options;
    private List<TileResult> _uncollapsedTilesSortedByEntropy = new();
    private PassOptions _passOptions;
    private MapOptions _mapOptions;
    private List<ITileConstraint> _tileConstraints = new();
    private IEnhancedRandom _random;

    public WaveFunctionCollapseGenerator()
    {
        _tileConstraints.Add(new AdapterConstraint());
        _tileConstraints.Add(new MandatoryAdapterConstraint());
        _tileConstraints.Add(new LimitConstraint());
        _tileConstraints.Add(new PlacementConstraint());
        _tileConstraints.Add(new OnlyAllowedIfNoValidTilesConstraint());
        _tileConstraints.Add(new CategoryConstraint());
        _tileConstraints.Add(new ProhibitedEmptyNeighbourConstraint());

        _tileConstraints.Sort((a, b) => a.Order - b.Order);
        TileChoices = new List<TileChoice>();
    }
    
    public void CreateTiles(Dictionary<string, Texture2D> textures, PassOptions passOptions, MapOptions mapOptions, IEnhancedRandom random)
    {
        _random = random;
        _mapOptions = mapOptions;
        _passOptions = passOptions;
        if (_passOptions.Options == null)
            _passOptions.Options = new GeneratorOptions();

        _options = passOptions.Options.Clone();

        _tileTemplates.Clear();

        foreach (var tile in passOptions.Tiles.Keys)
        {
            var tileTemplate = new TileTemplate(tile, passOptions.Tiles[tile], textures[tile]);

            _tileTemplates.Add(tileTemplate);

            tileTemplate.CreateTiles();
        }

        TileChoices.Clear();
        TileChoices.AddRange(_tileTemplates.SelectMany(t => t.TileChoices));
    }

    public void Clear()
    {
        _options = _passOptions.Options.Clone();
        _uncollapsedTilesSortedByEntropy.Clear();
        _tileChoicesPerPoint.Clear();

        Tiles = Array.Empty<TileResult>();
    }

    public void Prepare(IList<WaveFunctionCollapseGenerator> passes)
    {
        Clear();

        if (passes == null)
            passes = Array.Empty<WaveFunctionCollapseGenerator>();

        var mask = GetPriorLayerPointMask(passes);

        Tiles = new TileResult[MapWidth * MapHeight];

        for (var y = 0; y < MapHeight; y++)
        {
            for (var x = 0; x < MapWidth; x++)
            {
                var point = new Point(x, y);

                var tileResult = new TileResult(point);
                Tiles[point.ToIndex(MapWidth)] = tileResult;

                if (mask.Contains(point))
                    _uncollapsedTilesSortedByEntropy.Add(tileResult);
                else
                    tileResult.IsUnused = true;
            }
        }

        foreach (var tileResult in Tiles)
        {
            tileResult.SetNeighbours(Tiles, MapWidth, MapHeight);

            var entropyDivider = 2;

            foreach (var runFirstRule in _options.RunFirstRules)
            {
                if (tileResult.IsWithinRule(runFirstRule, tileResult.Point, _mapOptions))
                {
                    tileResult.Entropy -= Int32.MaxValue / entropyDivider;
                    tileResult.StartingEntropy = tileResult.Entropy;
                }

                entropyDivider++;
            }

            var tileChoices = _tileTemplates
                .SelectMany(t => t.TileChoices)
                .ToList();

            _tileChoicesPerPoint.Add(tileResult.Point, tileChoices);
        }

        foreach (var constraint in _tileConstraints)
        {
            constraint.Initialise(_tileTemplates, _mapOptions);
        }
    }

    private HashSet<Point> GetPriorLayerPointMask(IList<WaveFunctionCollapseGenerator> passes)
    {
        var mask = new HashSet<Point>(_mapOptions.MapWidth * _mapOptions.MapHeight);

        for (var y = 0; y < MapHeight; y++)
        {
            for (var x = 0; x < MapWidth; x++)
            {
                mask.Add(new Point(x, y));
            }
        }

        var priorPassMaskWorking = new HashSet<Point>(_mapOptions.MapWidth * _mapOptions.MapHeight);

        if (_options.PassMask != null && _options.PassMask.Any())
        {
            if (!passes.Any())
                throw new Exception("Reset must be called with the prior passes when using PassMask");

            foreach (var item in _options.PassMaskByPassIndex)
            {
                var pass = passes[item.Key];

                var acceptedPoints = pass.Tiles
                    .Where(t => t.ChosenTile != null)
                    .Where(t => item.Value.Contains(t.ChosenTile.TileTemplate.Name))
                    .Select(t => t.Point)
                    .ToList();

                foreach (var point in acceptedPoints)
                {
                    priorPassMaskWorking.Add(point);
                }
            }

            mask = mask.Intersect(priorPassMaskWorking).ToHashSet();
        }

        return mask;
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

        nextUncollapsedTile.ChosenTile = chosenTile;

        foreach (var constraint in _tileConstraints)
            constraint.AfterChoice(chosenTile);

        _uncollapsedTilesSortedByEntropy.Remove(nextUncollapsedTile);

        if (!_uncollapsedTilesSortedByEntropy.Any())
            return NextStepResult.Complete();

        ReduceEntropy(nextUncollapsedTile);

        return NextStepResult.Continue();
    }

    private void ReduceEntropy(TileResult chosenTile)
    {
        foreach (var neighbour in chosenTile.Neighbours.Where(t => !t.IsCollapsed && !t.IsUnused))
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
                tileToRecalculate.Entropy -= collapsedNeighbours.Sum(t => t.ChosenTile.Weight);
                break;
            case EntropyHeuristic.ReduceByCountAndMaxWeightOfNeighbours:
                tileToRecalculate.Entropy = tileToRecalculate.Entropy - collapsedNeighbours.Max(t => t.ChosenTile.Weight) - collapsedNeighbours.Count;
                break;
            case EntropyHeuristic.ReduceByCountOfAllTilesMinusPossibleTiles:
                var possibleTileChoices = GetPossibleTileChoices(tileToRecalculate);
                tileToRecalculate.Entropy -= (TileChoices.Count - possibleTileChoices.Count);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private TileResult GetNextUncollapsedTile(List<TileResult> entropy)
    {
        var lowestEntropy = entropy
            .TakeWhile(e => e.Entropy == entropy.First().Entropy)
            .ToList();

        var nextUncollapsedTile = lowestEntropy[_random.RandomIndex(lowestEntropy)];

        return nextUncollapsedTile;
    }

    private TileChoice ChooseTile(List<TileChoice> possibleTileChoices)
    {
        var sumWeights = possibleTileChoices.Sum(t => t.Weight);

        var randomNumber = _random.NextInt(0, sumWeights);

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

    private void RevertTilesInRadius(TileResult chosenTile)
    {
        var retryPoints = chosenTile.Point
            .NeighboursOutwardsFrom(_options.FallbackRadius, 0, MapWidth - 1, 0, MapHeight - 1)
            .OrderBy(p => (int)Distance.Manhattan.Calculate(p, chosenTile.Point))
            .ToList();

        foreach (var retryPoint in retryPoints)
        {
            var retryTile = Tiles[retryPoint.ToIndex(MapWidth)];

            if (retryTile.IsCollapsed)
            {
                var tileChoice = retryTile.ChosenTile;
                retryTile.ChosenTile = null;
                _uncollapsedTilesSortedByEntropy.Add(retryTile);

                foreach (var constraint in _tileConstraints)
                {
                    constraint.Revert(tileChoice);
                }
            }
        }

        foreach (var retryPoint in retryPoints)
        {
            var retryTile = Tiles[retryPoint.ToIndex(MapWidth)];

            RecalculateEntropy(retryTile);
        }

        RecalculateEntropy(chosenTile);

        _options.FallbackAttempts--;
        _options.FallbackRadius += _options.FallbackRadiusIncrement;
    }

    private List<TileChoice> GetPossibleTileChoices(TileResult chosenTile)
    {
        var validTiles = _tileChoicesPerPoint[chosenTile.Point].ToHashSet();

        if (!validTiles.Any())
            return new List<TileChoice>();

        foreach (var constraint in _tileConstraints)
        {
            foreach (var validTile in validTiles)
            {
                if (!constraint.Check(chosenTile, validTile, validTiles))
                    validTiles.Remove(validTile);
            }
        }
        
        return validTiles.ToList();
    }
}
