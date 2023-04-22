using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Graphics.Map;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.WaveFunctionCollapse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WaveFunctionCollapse.Wpf.MonoGameControls;

namespace WaveFunctionCollapse.Wpf
{
    public class MainWindowViewModel : MonoGameViewModel
    {
        private IGameCamera _gameCamera;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D _renderTarget;
        private WaveFunctionCollapseGenerator _waveFunctionCollapse;
        private MapEntity _mapEntity;

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

        public override void Initialize(IGameTimeService gameTimeService)
        {
            base.Initialize(gameTimeService);

            _gameCamera = new GameCamera(GraphicsDevice);
            _mapEntity = new MapEntity();
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            CreateRenderTarget();
            LoadWaveFunctionCollapse();
        }

        public void LoadWaveFunctionCollapse()
        {
            ResetPlayContinuously();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _mapFont = Content.Load<SpriteFont>("Fonts/MapFont");

            _waveFunctionCollapse = new WaveFunctionCollapseGenerator();
            _waveFunctionCollapse.CreateTiles(Content, "WaveFunctionCollapse/Dancing");

            _mapWidth = 30;
            _mapHeight = 30;
            _tileWidth = 60;
            _tileHeight = 60;

            ResetWaveFunctionCollapse();
        }

        private void ResetPlayContinuously()
        {
            _playContinuously = false;
            _playUntilComplete = false;
            _dateTimeFinished = null;
            GameTimeService.Reset();
            GameTimeService.Start();
        }

        private void ResetWaveFunctionCollapse()
        {
            _waveFunctionCollapse.Reset(new WaveFunctionCollapseGeneratorOptions(_mapWidth, _mapHeight)
                { EntropyCalculationMethod = EntropyCalculationMethod.ReduceByCountAndMaxWeightOfNeighbours, FallbackAttempts = 99, FallbackRadiusIncrement = 0, FallbackRadius = 3});
        }

        public override void Update()
        {
            if (!_dateTimeFinished.HasValue && (_playUntilComplete || _playContinuously))
            {
                var result = _waveFunctionCollapse.ExecuteNextStep();

                if (result.IsComplete)
                    _playUntilComplete = false;

                if (result.IsComplete || result.IsFailed)
                    _dateTimeFinished = GameTimeService.GameTime.TotalRealTime;
            }

            if (_dateTimeFinished.HasValue)
            {
                if (GameTimeService.GameTime.TotalGameTime - _dateTimeFinished > _secondsForNextIteration)
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
                GraphicsDevice,
                _tileWidth * _mapWidth,
                _tileHeight * _mapHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                GraphicsDevice.PresentationParameters.DepthStencilFormat,
                0,
                RenderTargetUsage.PreserveContents
            );

            _mapEntity.Initialize(GraphicsDevice.Viewport.Height / _mapHeight, GraphicsDevice.Viewport.Height / _mapHeight);
            _mapEntity.LoadContent(_mapWidth, _mapHeight);
            _mapEntity.SetMapTexture(_renderTarget);
        }

        public override void Draw()
        {
            var oldRenderTargets = GraphicsDevice.GetRenderTargets();

            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.DarkGray);

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
                        SpriteEffects.None,
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

            GraphicsDevice.RestoreGraphicsDeviceAfterSpriteBatchDraw();
            GraphicsDevice.SetRenderTargets(oldRenderTargets);

            _mapEntity.Transform.ChangeTranslation(new Vector3(0, 0, -GraphicsDevice.ViewportRectangle().Height / 2f));

            // _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);
            // _spriteBatch.Draw(_renderTarget, _renderTarget.Bounds, Color.White);
            // _spriteBatch.End();
            // The "Bleeding"/anti-aliasing of texture edges seems to only happen when the texture is drawn on the quad as the above commented code has no bleeding
            _mapEntity.Draw(_gameCamera.View, _gameCamera.Projection, _mapEntity.Transform.Transform);
        }
    }
}