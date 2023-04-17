using System.Threading;
using System.Threading.Tasks;
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
    private readonly IGameTimeService _gameTimeService;
    private Panel _leftPanel;
    private Panel _mapPanel;
    private SpriteBatch _spriteBatch;
    private WaveFunctionCollapseGenerator _waveFunctionCollapse;
    private int _mapHeight;
    private int _mapWidth;
    private bool _playUntilComplete;
    private bool _playContinuously;
    private TimeSpan? _dateTimeFinished;
    private TimeSpan _secondsForNextIteration = TimeSpan.FromSeconds(2);

    public WaveFunctionCollapseView(
        WaveFunctionCollapseViewModel waveFunctionCollapseViewModel,
        IGameTimeService gameTimeService)
        : base(waveFunctionCollapseViewModel)
    {
        _gameTimeService = gameTimeService;
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

        _waveFunctionCollapse = new WaveFunctionCollapseGenerator();
        _waveFunctionCollapse.CreateTiles(GameProvider.Game.Content);

        _mapWidth = 30;
        _mapHeight = 30;

        _waveFunctionCollapse.Reset(_mapWidth, _mapHeight);
    }

    public override void Draw()
    {
        var tileSize = new Vector2(60, 60);
        var drawOrigin = new Vector2(1000, 100) + tileSize;

        _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);

        var tiles = _waveFunctionCollapse.CurrentState;

        foreach (var tile in tiles)
        {
            if (tile.IsCollapsed)
            {
                var offset = tileSize * new Vector2(tile.Point.X, tile.Point.Y);
                var drawLocation = drawOrigin + offset;
                var rotateOrigin = tile.TileChoice.Texture.Bounds.Size.ToVector2() / 2;

                var destinationRectangle = new Rectangle((int)drawLocation.X, (int)drawLocation.Y, (int)tileSize.X, (int)tileSize.Y);

                _spriteBatch.Draw(tile.TileChoice.Texture,
                    destinationRectangle,
                    tile.TileChoice.Texture.Bounds,
                    Color.White,
                    tile.TileChoice.Rotation,
                    rotateOrigin,
                    SpriteEffects.None,
                    0);
            }
        }

        _spriteBatch.End();
    }

    public Task<Unit> Handle(NextStepRequest request, CancellationToken cancellationToken)
    {
        ResetPlayContinuously();

        _waveFunctionCollapse.NextStep();
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

        _waveFunctionCollapse.Reset(_mapWidth, _mapHeight);

        return Unit.Task;
    }

    public override void Update()
    {
        if (!_dateTimeFinished.HasValue && (_playUntilComplete || _playContinuously))
        {
            var result = _waveFunctionCollapse.NextStep();

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
                _waveFunctionCollapse.Reset(_mapWidth, _mapHeight);
            }
        }

        base.Update();
    }
}
