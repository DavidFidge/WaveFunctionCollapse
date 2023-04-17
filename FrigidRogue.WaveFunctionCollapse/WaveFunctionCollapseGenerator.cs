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
    private readonly List<TileChoice> _tiles = new();
    private int _mapWidth;
    private int _mapHeight;
    public TileResult[] CurrentState { get; private set; }
    public List<TileChoice> Tiles => _tiles;

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
            var tile = new TileContent
            {
                Name = texture.Key,
                Texture = texture.Value,
                Attributes = tileAttributes.Tiles[texture.Key]
            };

            _tiles.AddRange(tile.CreateTiles());
        }
    }

    public void Reset(int mapWidth, int mapHeight)
    {
        _mapHeight = mapHeight;
        _mapWidth = mapWidth;

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