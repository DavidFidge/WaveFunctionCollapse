using System.Windows;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WaveFunctionCollapse.Wpf.MonoGameControls;

public interface IMonoGameViewModel : IDisposable
{
    IGraphicsDeviceService GraphicsDeviceService { get; set; }

    void Initialize(IGameTimeService gameTimeService);
    void LoadContent();
    void UnloadContent();
    void Update();
    void Draw();
    void OnActivated(object sender, EventArgs args);
    void OnDeactivated(object sender, EventArgs args);
    void OnExiting(object sender, EventArgs args);

    void SizeChanged(object sender, SizeChangedEventArgs args);
}