using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Graphics.Map;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;

using MediatR;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WaveFunctionCollapse.Messages;
using WaveFunctionCollapse.ViewModels;

namespace WaveFunctionCollapse.Views;

public class WaveFunctionCollapseView : BaseView<WaveFunctionCollapseViewModel, WaveFunctionCollapseData>,
    IRequestHandler<NextStepRequest>,
    IRequestHandler<NewMapRequest>,
    IRequestHandler<PlayContinuouslyRequest>,
    IRequestHandler<PlayUntilCompleteRequest>,
    IRequestHandler<ChangeContentRequest>,
    IRequestHandler<ReloadJsonRequest>
{
    private readonly IGameCamera _gameCamera;
    private readonly MapEntity _mapEntity;
    private RenderTarget2D _renderTarget;
    private SpriteBatch _spriteBatch;
    private readonly IGameTimeService _gameTimeService;
    private Panel _leftPanel;
    private Panel _mapPanel;
    private bool _playUntilComplete;
    private bool _playContinuously;
    private TimeSpan? _dateTimeFinished;
    private TimeSpan _secondsForNextIteration = TimeSpan.FromSeconds(2);
    private SpriteFont _mapFont;
    private bool _hasUpdated;
    private DropDown _tileSetDropDown;
    private string _startingTileSet;

    public WaveFunctionCollapseView(
        WaveFunctionCollapseViewModel waveFunctionCollapseViewModel,
        IGameTimeService gameTimeService,
        IGameCamera gameCamera,
        MapEntity mapEntity)
        : base(waveFunctionCollapseViewModel)
    {
        _gameTimeService = gameTimeService;
        _gameCamera = gameCamera;
        _mapEntity = mapEntity;
    }

    protected override void InitializeInternal()
    {
        _leftPanel = new Panel()
            .Anchor(Anchor.TopLeft)
            .Width(0.19f)
            .SkinNone()
            .NoPadding()
            .Height(0.999f);

        _mapPanel = new Panel()
            .Anchor(Anchor.TopRight)
            .Width(0.805f)
            .SkinNone()
            .NoPadding()
            .Height(0.999f);

        _tileSetDropDown = new DropDown()
            .Height(800)
            .Width(400)
            .AddTo(_leftPanel);

        _tileSetDropDown.DefaultText = "Select Tile Set";

        var rulesFiles = GameProvider.Game.Content.Load<string[]>("Content")
            .Where(a => a.ToLower().StartsWith("wavefunctioncollapse"))
            .Select(a => a.Split('/').Skip(1).Take(1).First())
            .Distinct()
            .ToList();

        foreach (var rule in rulesFiles)
        {
            _tileSetDropDown.AddItem(rule);
        }

        _tileSetDropDown.OnValueChange += e =>
        {
            Mediator.Send(new ChangeContentRequest { Content = GetSelectedContent(e)});
        };

        new Button("New Map")
            .SendOnClick<NewMapRequest>(Mediator)
            .AddTo(_leftPanel);

        new Button("Reload json")
            .SendOnClick<ReloadJsonRequest>(Mediator)
            .AddTo(_leftPanel);

        new Button("Next Step")
            .SendOnClick<NextStepRequest>(Mediator)
            .AddTo(_leftPanel);

        new Button("Play Until Complete")
            .SendOnClick<PlayUntilCompleteRequest>(Mediator)
            .AddTo(_leftPanel);

        new Button("Play Continuously")
            .SendOnClick<PlayContinuouslyRequest>(Mediator)
            .AddTo(_leftPanel);

        new Button("Exit")
            .SendOnClick<QuitToDesktopRequest>(Mediator)
            .AddTo(_leftPanel);

        RootPanel.AddChild(_leftPanel);
        RootPanel.AddChild(_mapPanel);
        
        if (!string.IsNullOrEmpty(_startingTileSet))
        {
            _tileSetDropDown.SelectedValue = _startingTileSet;
        }
    }

    private static string GetSelectedContent(Entity e)
    {
        return $"WaveFunctionCollapse/{((DropDown)e).SelectedValue}";
    }

    private void ResetPlayContinuously()
    {
        _playContinuously = false;
        _playUntilComplete = false;
        _dateTimeFinished = null;
        _gameTimeService.Reset();
        _gameTimeService.Start();
    }

    public void Initialise(string startingTileSet)
    {
        _startingTileSet = startingTileSet;
        ResetPlayContinuously();

        _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        _mapFont = GameProvider.Game.Content.Load<SpriteFont>("Fonts/MapFont");
    }

    public override void Draw()
    {
        if (!_hasUpdated)
            return;

        _hasUpdated = false;

        if (_renderTarget == null)
        {
            Game.GraphicsDevice.Clear(Color.Black);
            return;
        }

        var oldRenderTargets = Game.GraphicsDevice.GetRenderTargets();

        Game.GraphicsDevice.SetRenderTarget(_renderTarget);
        Game.GraphicsDevice.Clear(Color.DarkGray);

        _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);

        var tiles = _viewModel.GetAllTiles();

        foreach (var tile in tiles)
        {
            if (tile.IsCollapsed)
            {
                var offset = _viewModel.TileSize * new Vector2(tile.Point.X, tile.Point.Y);
                var rotateOrigin = tile.ChosenTile.Texture.Bounds.Size.ToVector2() / 2f;
                var position = (_viewModel.TileSize / 2f) + offset;

                _spriteBatch.Draw(tile.ChosenTile.Texture,
                    position,
                    tile.ChosenTile.Texture.Bounds,
                    Color.White,
                    tile.ChosenTile.Rotation,
                    rotateOrigin,
                    (float)_viewModel.TileWidth / tile.ChosenTile.Texture.Width,
                    tile.ChosenTile.SpriteEffects,
                    0);
            }
        }
        _spriteBatch.End();
        _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);

        foreach (var tile in _viewModel.GetCurrentTiles().Where(t => !t.IsCollapsed))
        {
            var position = _viewModel.TileSize * new Vector2(tile.Point.X, tile.Point.Y);

            position.X += _viewModel.TileSize.X / 4f;
            position.Y += _viewModel.TileSize.Y / 2f;

            _spriteBatch.DrawString(_mapFont, tile.Entropy.ToString(), position, Color.Black);
        }

        _spriteBatch.End();

        Game.GraphicsDevice.RestoreGraphicsDeviceAfterSpriteBatchDraw();
        Game.GraphicsDevice.SetRenderTargets(oldRenderTargets);

        _mapEntity.Transform.ChangeTranslation(new Vector3(0, 0, -Game.CustomGraphicsDeviceManager.PreferredBackBufferHeight / 2f));

        // _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);
        // _spriteBatch.Draw(_renderTarget, _renderTarget.Bounds, Color.White);
        // _spriteBatch.End();
        // The "Bleeding"/anti-aliasing of texture edges seems to only happen when the texture is drawn on the quad as the above commented code has no bleeding
        _mapEntity.Draw(_gameCamera.View, _gameCamera.Projection, _mapEntity.Transform.Transform);
    }

    public Task<Unit> Handle(NextStepRequest request, CancellationToken cancellationToken)
    {
        ResetPlayContinuously();

        _viewModel.ExecuteNextStep();
        return Unit.Task;
    }

    public Task<Unit> Handle(PlayUntilCompleteRequest request, CancellationToken cancellationToken)
    {
        _playContinuously = false;
        _playUntilComplete = !_playUntilComplete;

        if (!_playUntilComplete)
            ResetPlayContinuously();

        return Unit.Task;
    }

    public Task<Unit> Handle(PlayContinuouslyRequest request, CancellationToken cancellationToken)
    {
        _playUntilComplete = false;
        _playContinuously = !_playContinuously;

        if (!_playContinuously)
            ResetPlayContinuously();

        return Unit.Task;
    }

    public Task<Unit> Handle(NewMapRequest request, CancellationToken cancellationToken)
    {
        ResetPlayContinuously();

        _viewModel.Reset();

        return Unit.Task;
    }

    public override void Update()
    {
        if (_renderTarget == null)
            return;

        _hasUpdated = true;

        if (!_dateTimeFinished.HasValue && (_playUntilComplete || _playContinuously))
        {
            var result = _viewModel.ExecuteNextStep();

            if (result.IsComplete)
                _playUntilComplete = false;

            if (result.IsComplete || result.IsFailed)
                _dateTimeFinished = _gameTimeService.GameTime.TotalRealTime;
        }

        if (_dateTimeFinished.HasValue)
        {
            if (_gameTimeService.GameTime.TotalGameTime - _dateTimeFinished > _secondsForNextIteration)
            {
                _dateTimeFinished = null;
                _viewModel.Reset();
            }
        }

        _gameCamera.Update();

        base.Update();
    }

    protected void CreateRenderTarget()
    {
        _renderTarget = new RenderTarget2D(
            Game.GraphicsDevice,
            _viewModel.TileWidth * _viewModel.MapWidth,
            _viewModel.TileHeight * _viewModel.MapHeight,
            false,
            Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
            Game.GraphicsDevice.PresentationParameters.DepthStencilFormat,
            0,
            RenderTargetUsage.PreserveContents
        );

        _mapEntity.Initialize(Game.GraphicsDevice.Viewport.Height / _viewModel.MapHeight, Game.GraphicsDevice.Viewport.Height / _viewModel.MapHeight);
        _mapEntity.LoadContent(_viewModel.MapWidth, _viewModel.MapHeight);
        _mapEntity.SetMapTexture(_renderTarget);
    }

    public Task<Unit> Handle(ChangeContentRequest request, CancellationToken cancellationToken)
    {
        _renderTarget = null;
        _viewModel.CreatePasses(GameProvider.Game.Content, request.Content);
        CreateRenderTarget();

        return Unit.Task;
    }

    public Task<Unit> Handle(ReloadJsonRequest request, CancellationToken cancellationToken)
    {
        ResetPlayContinuously();
        _viewModel.CreatePasses(GameProvider.Game.Content, GetSelectedContent(_tileSetDropDown));

        return Unit.Task;
    }
}
