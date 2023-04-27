using FrigidRogue.WaveFunctionCollapse.Options;
using GoRogue.Random;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;

namespace FrigidRogue.WaveFunctionCollapse;

public class WaveFunctionCollapseGeneratorPasses
{
    public MapOptions MapOptions => _rules.MapOptions;

    private List<WaveFunctionCollapseGenerator> _generators = new();
    private Rules _rules;

    private Queue<WaveFunctionCollapseGenerator> _generatorsQueue = new();
    private WaveFunctionCollapseGenerator _currentGenerator;
    private Dictionary<string, Texture2D> _textures;

    public int TileWidth => _textures.First().Value.Bounds.Width * MapOptions.TileSizeMultiplier;
    public int TileHeight => _textures.First().Value.Bounds.Height * MapOptions.TileSizeMultiplier;

    public int MapWidth => MapOptions.MapWidth;
    public int MapHeight => MapOptions.MapHeight;

    public void CreatePasses(ContentManager contentManager, string contentPath)
    {
        _rules = contentManager.Load<Rules>($"{contentPath}/Rules.json",
            new JsonContentLoader());

        var assetsList = contentManager.Load<string[]>("Content");

        _textures = assetsList
            .Where(a => a.StartsWith($"{contentPath}/") && !a.EndsWith(".json"))
            .ToDictionary(
                a => a.Split("/").Last().Replace(".png", ""),
                a => contentManager.Load<Texture2D>(a));

        CreatePasses(_rules, _textures);
    }

    public void CreatePasses(Rules rules, Dictionary<string, Texture2D> textures)
    {
        for (var index = 0; index < rules.Passes.Length; index++)
        {
            var layer = rules.Passes[index];
            var generator = new WaveFunctionCollapseGenerator();
            generator.CreateTiles(textures, layer, rules.MapOptions, GlobalRandom.DefaultRNG);
            _generators.Add(generator);
        }
    }

    public void Reset()
    {
        foreach (var generator in _generators)
        {
            generator.Clear();
            _generatorsQueue.Enqueue(generator);
        }

        _currentGenerator = _generatorsQueue.Dequeue();
        _currentGenerator.Prepare(null);
    }

    public NextStepResult Execute()
    {
        if (_currentGenerator == null)
            Reset();

        var result = NextStepResult.Continue();

        while (result.IsContinue)
        {
            result = _currentGenerator.Execute();

            if (result.IsComplete && _generatorsQueue.Any())
            {
                _currentGenerator = _generatorsQueue.Dequeue();
                _currentGenerator.Prepare(_generators);
                result = NextStepResult.Continue();
            }
        }

        _currentGenerator = null;

        return result;
    }

    public NextStepResult ExecuteNextStep()
    {
        if (_currentGenerator == null)
            Reset();

        var result = _currentGenerator.ExecuteNextStep();

        if (result.IsComplete && _generatorsQueue.Any())
        {
            _currentGenerator = _generatorsQueue.Dequeue();
            _currentGenerator.Prepare(_generators);

            result = NextStepResult.Continue();
        }

        if (!result.IsContinue)
            _currentGenerator = null;

        return result;
    }

    public IEnumerable<TileResult> GetAllTiles()
    {
        return _generators.SelectMany(generator => generator.Tiles);
    }

    public IEnumerable<TileResult> GetCurrentTiles()
    {
        return _currentGenerator?.Tiles ?? Array.Empty<TileResult>();
    }
}