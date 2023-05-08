using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;

namespace FrigidRogue.WaveFunctionCollapse.ContentLoaders;

public class WaveFunctionCollapseGeneratorPassesContentLoader : IWaveFunctionCollapseGeneratorPassesContentLoader
{
    private readonly ContentManager _contentManager;
    private Rules _rules;
    private Dictionary<string, Texture2D> _textures;

    public Rules Rules => _rules;
    public Dictionary<string, Texture2D> Textures => _textures;

    public WaveFunctionCollapseGeneratorPassesContentLoader(ContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    public void LoadContent(string content)
    {
        _rules = _contentManager.Load<Rules>($"{content}/Rules.json",
            new JsonContentLoader());

        var assetsList = _contentManager.Load<string[]>("Content");

        _textures = assetsList
            .Where(a => a.ToLower().StartsWith($"{content.ToLower()}/") && !a.EndsWith(".json"))
            .ToDictionary(
                a => a.Split("/").Last().Replace(".png", ""),
                a => _contentManager.Load<Texture2D>(a));
    }
}