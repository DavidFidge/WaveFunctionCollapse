﻿using FrigidRogue.WaveFunctionCollapse.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrigidRogue.WaveFunctionCollapse.Renderers;

public class WaveFunctionCollapseGeneratorPassesRenderer : IWaveFunctionCollapseGeneratorPassesRenderer
{
    private readonly GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _renderTarget;
    private Rectangle _tileTextureSize;
    private Vector2 _tileTextureSizeVector2 => _tileTextureSize.Size.ToVector2();
    private MapOptions _mapOptions;

    private Rectangle _mapTextureSize => new Rectangle(0, 0, _mapWidth * _tileTextureSize.Size.X,
        _mapHeight * _tileTextureSize.Size.Y);

    private int _mapWidth => _mapOptions.MapWidth;
    private int _mapHeight => _mapOptions.MapHeight;

    public WaveFunctionCollapseGeneratorPassesRenderer(
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch = null,
        RenderTarget2D renderTarget = null)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch ?? new SpriteBatch(graphicsDevice);
        _renderTarget = renderTarget ?? new RenderTarget2D(graphicsDevice, _mapTextureSize.Width, _mapTextureSize.Height);
    }

    public Texture2D RenderToTexture2D(TileResult[] tiles, MapOptions mapOptions, Rectangle tileTextureSize)
    {
        _mapOptions = mapOptions;
        _tileTextureSize = tileTextureSize;

        var oldRenderTargets = _graphicsDevice.GetRenderTargets();

        _graphicsDevice.SetRenderTarget(_renderTarget);

        _graphicsDevice.Clear(Color.Transparent);

        _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);

        foreach (var tile in tiles)
        {
            if (tile.IsCollapsed)
            {
                var offset = _tileTextureSizeVector2 * new Vector2(tile.Point.X, tile.Point.Y);
                var rotateOrigin = _tileTextureSizeVector2 / 2f;
                var position = (_tileTextureSizeVector2) + offset;

                _spriteBatch.Draw(tile.ChosenTile.Texture,
                    position,
                    tile.ChosenTile.Texture.Bounds,
                    Color.White,
                    tile.ChosenTile.Rotation,
                    rotateOrigin,
                    _tileTextureSizeVector2.X / tile.ChosenTile.Texture.Width,
                    tile.ChosenTile.SpriteEffects,
                    0);
            }
        }

        _spriteBatch.End();
        _graphicsDevice.SetRenderTargets(oldRenderTargets);

        return _renderTarget;
    }
}