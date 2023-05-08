using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Graphics;

namespace FrigidRogue.WaveFunctionCollapse.ContentLoaders;

public interface IWaveFunctionCollapseGeneratorPassesContentLoader
{
    public Rules Rules { get; }
    public Dictionary<string, Texture2D> Textures { get; }
    void LoadContent(string content);
}