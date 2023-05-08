using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using FrigidRogue.WaveFunctionCollapse.Options;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using GoRogue.Random;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrigidRogue.WaveFunctionCollapse;

public class WaveFunctionCollapseGeneratorPasses : IWaveFunctionCollapseGeneratorPasses
{
    public MapOptions MapOptions
    {
        get => _rules.MapOptions;
        set => _rules.MapOptions = value;
    }

    private List<WaveFunctionCollapseGenerator> _generators = new();
    private Rules _rules;

    private Queue<WaveFunctionCollapseGenerator> _generatorsQueue = new();
    private WaveFunctionCollapseGenerator _currentGenerator;
    private Dictionary<string, Texture2D> _textures;

    public Rectangle TileTextureSize => _textures.First().Value.Bounds;

    public int TileWidth => _textures.First().Value.Bounds.Width * MapOptions.TileSizeMultiplier;

    public int TileHeight => _textures.First().Value.Bounds.Height * MapOptions.TileSizeMultiplier;

    public int MapWidth => MapOptions.MapWidth;
    public int MapHeight => MapOptions.MapHeight;

    public void LoadContent(IWaveFunctionCollapseGeneratorPassesContentLoader contentLoader, string contentPath)
    {
        contentLoader.LoadContent(contentPath);
        _rules = contentLoader.Rules;
        _textures = contentLoader.Textures;

        CreatePasses(_rules, _textures);
    }

    public void CreatePasses()
    {
        CreatePasses(_rules, _textures);
    }

    public void CreatePasses(Rules rules, Dictionary<string, Texture2D> textures)
    {
        _generatorsQueue.Clear();
        _generators.Clear();
        _currentGenerator = null;

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
        _generatorsQueue.Clear();

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

    public NextStepResult ExecuteUntilSuccess()
    {
        var nextStepResult = NextStepResult.Continue();

        while (!nextStepResult.IsComplete)
        {
            nextStepResult = Execute();
        }

        return nextStepResult;
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

    public Texture2D RenderToTexture2D(IWaveFunctionCollapseGeneratorPassesRenderer renderer)
    {
        return renderer.RenderToTexture2D(GetAllTiles().ToArray(), MapOptions, TileTextureSize);
    }
}