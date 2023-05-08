using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.ContentLoaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using WaveFunctionCollapse.Messages;

namespace WaveFunctionCollapse.ViewModels;

public class WaveFunctionCollapseViewModel : BaseViewModel<WaveFunctionCollapseData>
{
    private WaveFunctionCollapseGeneratorPasses _waveFunctionCollapsePasses;
    public int TileWidth => _waveFunctionCollapsePasses.TileWidth;
    public int TileHeight => _waveFunctionCollapsePasses.TileHeight;
    
    public int MapWidth => _waveFunctionCollapsePasses.MapWidth;
    public int MapHeight => _waveFunctionCollapsePasses.MapHeight;
    public Vector2 TileSize => new Vector2(TileWidth, TileHeight);
    public bool ExecuteAllSteps { get; set; }

    public NextStepResult Execute()
    {
        if (ExecuteAllSteps)
            return _waveFunctionCollapsePasses.Execute();

        return _waveFunctionCollapsePasses.ExecuteNextStep();
    }

    public void LoadContent(ContentManager gameContent, string contentName)
    {
        _waveFunctionCollapsePasses = new WaveFunctionCollapseGeneratorPasses();
        var contentLoader =
            new WaveFunctionCollapseGeneratorPassesContentLoader(gameContent);
        _waveFunctionCollapsePasses.LoadContent(contentLoader, contentName);
    }

    public void CreatePasses()
    {
        _waveFunctionCollapsePasses.CreatePasses();
        Reset();
    }

    public IEnumerable<TileResult> GetAllTiles() => _waveFunctionCollapsePasses.GetAllTiles();
    public IEnumerable<TileResult> GetCurrentTiles() => _waveFunctionCollapsePasses.GetCurrentTiles();

    public void Reset()
    {
        _waveFunctionCollapsePasses.Reset();
    }

    public void ChangeMapDimensions(int mapWidth, int mapHeight)
    {
        _waveFunctionCollapsePasses.MapOptions.MapWidth = mapWidth;
        _waveFunctionCollapsePasses.MapOptions.MapHeight = mapHeight;

        _waveFunctionCollapsePasses.CreatePasses();

        Mediator.Send(new MapDimensionsChangedRequest());
        Mediator.Send(new NewMapRequest());
    }
}