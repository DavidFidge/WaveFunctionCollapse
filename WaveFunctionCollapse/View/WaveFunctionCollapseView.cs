using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Graphics.Map;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Extensions;
using FrigidRogue.WaveFunctionCollapse;
using GeonBit.UI.Entities;

using MediatR;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WaveFunctionCollapse.Messages;
using WaveFunctionCollapse.ViewModels;

namespace WaveFunctionCollapse.Views;

public class WaveFunctionCollapseView : BaseView<WaveFunctionCollapseViewModel, WaveFunctionCollapseData>,
    IRequestHandler<NextStepRequest>,
    IRequestHandler<MewMapRequest>,
    IRequestHandler<PlayContinuouslyRequest>,
    IRequestHandler<PlayUntilCompleteRequest>
{
    private readonly IGameCamera _gameCamera;
    private readonly MapEntity _mapEntity;
    private RenderTarget2D _renderTarget;
    private SpriteBatch _spriteBatch;
    private readonly IGameTimeService _gameTimeService;
    private Panel _leftPanel;
    private Panel _mapPanel;

    private WaveFunctionCollapseGenerator _waveFunctionCollapse;

    private int _mapHeight;
    private int _mapWidth;
    private int _tileWidth;
    private int _tileHeight;
    private Vector2 _tileSize => new Vector2(_tileWidth, _tileHeight);

    private bool _playUntilComplete;
    private bool _playContinuously;
    private TimeSpan? _dateTimeFinished;
    private TimeSpan _secondsForNextIteration = TimeSpan.FromSeconds(2);
    private SpriteFont _mapFont;

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

        new Button("New Map")
            .SendOnClick<MewMapRequest>(Mediator)
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

        CreateRenderTarget();
    }

    private void ResetPlayContinuously()
    {
        _playContinuously = false;
        _playUntilComplete = false;
        _dateTimeFinished = null;
        _gameTimeService.Reset();
        _gameTimeService.Start();
    }

    public void LoadWaveFunctionCollapse()
    {
        ResetPlayContinuously();

        _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        _mapFont = GameProvider.Game.Content.Load<SpriteFont>("Fonts/MapFont");

        _waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        _waveFunctionCollapse.CreateTiles(GameProvider.Game.Content, "WaveFunctionCollapse/Dancing");

        _mapWidth = 30;
        _mapHeight = 30;
        _tileWidth = 96;
        _tileHeight = 96;

        ResetWaveFunctionCollapse();
    }

    private void ResetWaveFunctionCollapse()
    {
        _waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(_mapWidth, _mapHeight)
            { EntropyCalculationMethod = EntropyCalculationMethod.ReduceByCountAndMaxWeightOfNeighbours, FallbackAttempts = 99, FallbackRadiusIncrement = 0, FallbackRadius = 3});
    }

    public override void Draw()
    {
        var oldRenderTargets = Game.GraphicsDevice.GetRenderTargets();

        Game.GraphicsDevice.SetRenderTarget(_renderTarget);
        Game.GraphicsDevice.Clear(Color.DarkGray);

        _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);

        var tiles = _waveFunctionCollapse.CurrentState;

        foreach (var tile in tiles)
        {
            if (tile.IsCollapsed)
            {
                var offset = _tileSize * new Vector2(tile.Point.X, tile.Point.Y);
                var rotateOrigin = tile.TileChoice.Texture.Bounds.Size.ToVector2() / 2f;
                var position = (_tileSize / 2f) + offset;

                _spriteBatch.Draw(tile.TileChoice.Texture,
                    position,
                    tile.TileChoice.Texture.Bounds,
                    Color.White,
                    tile.TileChoice.Rotation,
                    rotateOrigin,
                    (float)_tileWidth / tile.TileChoice.Texture.Width,
                    tile.TileChoice.SpriteEffects,
                    0);
            }
            else
            {
                var position = _tileSize * new Vector2(tile.Point.X, tile.Point.Y);

                position.X += _tileSize.X / 4f;
                position.Y += _tileSize.Y / 2f;

                _spriteBatch.DrawString(_mapFont, tile.Entropy.ToString(), position, Color.Black);
            }
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

        _waveFunctionCollapse.ExecuteNextStep();
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

    public Task<Unit> Handle(MewMapRequest request, CancellationToken cancellationToken)
    {
        _playContinuously = false;
        _playUntilComplete = false;
        _dateTimeFinished = null;

        ResetWaveFunctionCollapse();

        return Unit.Task;
    }

    public override void Update()
    {
        if (!_dateTimeFinished.HasValue && (_playUntilComplete || _playContinuously))
        {
            var result = _waveFunctionCollapse.ExecuteNextStep();

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
                ResetWaveFunctionCollapse();
            }
        }

        _gameCamera.Update();

        base.Update();
    }

    protected void CreateRenderTarget()
    {
        _renderTarget = new RenderTarget2D(
            Game.GraphicsDevice,
            _tileWidth * _mapWidth,
            _tileHeight * _mapHeight,
            false,
            Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
            Game.GraphicsDevice.PresentationParameters.DepthStencilFormat,
            0,
            RenderTargetUsage.PreserveContents
        );

        _mapEntity.Initialize(Game.GraphicsDevice.Viewport.Height / _mapHeight, Game.GraphicsDevice.Viewport.Height / _mapHeight);
        _mapEntity.LoadContent(_mapWidth, _mapHeight);
        _mapEntity.SetMapTexture(_renderTarget);
    }
}
