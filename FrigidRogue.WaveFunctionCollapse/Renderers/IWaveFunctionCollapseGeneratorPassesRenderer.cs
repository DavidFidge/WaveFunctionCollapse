using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrigidRogue.WaveFunctionCollapse.Renderers;

public interface IWaveFunctionCollapseGeneratorPassesRenderer
{
    public Texture2D RenderToTexture2D(TileResult[] tiles, MapOptions mapOptions, Rectangle tileTextureSize);
}