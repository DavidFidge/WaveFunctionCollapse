using GoRogue.Random;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using ShaiRandom.Generators;

using Point = SadRogue.Primitives.Point;

namespace FrigidRogue.WaveFunctionCollapse;

public class WaveFunctionCollapseGenerator
{
    private List<TileChoice> _tiles;
    private int _mapWidth;
    private int _mapHeight;
    public TileResult[] CurrentState { get; private set; }
    public List<TileChoice> Tiles => _tiles;

    private Dictionary<Point, List<TileChoice>> _tileChoicesPerPoint = new();
    private List<TileContent> _tileContent = new();

    public void CreateTiles(ContentManager contentManager, string contentPath)
    {
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

        _tiles = _tileContent.SelectMany(t => t.TileChoices).ToList();
    }

    public void Reset(int mapWidth, int mapHeight)
    {
        _mapHeight = mapHeight;
        _mapWidth = mapWidth;
        _tileChoicesPerPoint.Clear();

        CurrentState = new TileResult[mapWidth * mapHeight];

        for (var y = 0; y < mapHeight; y++)
        {
            for (var x = 0; x < mapWidth; x++)
            {
                var point = new Point(x, y);

                CurrentState[point.ToIndex(mapWidth)] = new TileResult(point, mapWidth);
            }
        }

        foreach (var tileResult in CurrentState)
        {
            tileResult.SetNeighbours(CurrentState, _mapWidth, _mapHeight);

            var initialisationSelection = _tileContent
                .Where(t => t.IsWithinInitialisationRule(tileResult.Point, _mapWidth, _mapHeight))
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
        var entropy = CurrentState
            .Where(t => !t.IsCollapsed)
            .OrderBy(t => t.Entropy)
            .ToList();

        if (!entropy.Any())
            return NextStepResult.Complete(); 

        entropy = entropy.TakeWhile(e => e.Entropy == entropy.First().Entropy).ToList();

        var chosenTile = entropy[GlobalRandom.DefaultRNG.RandomIndex(entropy)];

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

        if (!validTiles.Any())
            return NextStepResult.Failed();

        var sumWeights = validTiles.Sum(t => t.Weight);

        var randomNumber = GlobalRandom.DefaultRNG.NextInt(0, sumWeights);

        var i = 0;
        while (randomNumber > 0)
        {
            randomNumber -= validTiles[i].Weight;

            if (randomNumber < 0)
                break;

            i++;
        }

        chosenTile.SetTile(validTiles[i]);

        return NextStepResult.Continue();
    }
}