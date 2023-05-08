using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using FrigidRogue.WaveFunctionCollapse.Options;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrigidRogue.WaveFunctionCollapse;

public interface IWaveFunctionCollapseGeneratorPasses
{
    MapOptions MapOptions { get; set; }
    Rectangle TileTextureSize { get; }
    int TileWidth { get; }
    int TileHeight { get; }
    int MapWidth { get; }
    int MapHeight { get; }
    void LoadContent(IWaveFunctionCollapseGeneratorPassesContentLoader contentLoader, string contentPath);
    void CreatePasses();
    void CreatePasses(Rules rules, Dictionary<string, Texture2D> textures);
    void Reset();
    NextStepResult Execute();
    NextStepResult ExecuteUntilSuccess();
    NextStepResult ExecuteNextStep();
    IEnumerable<TileResult> GetAllTiles();
    IEnumerable<TileResult> GetCurrentTiles();
    Texture2D RenderToTexture2D(IWaveFunctionCollapseGeneratorPassesRenderer renderer);
}