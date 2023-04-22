using System.Security.Cryptography;
using System.Windows;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WaveFunctionCollapse.Wpf.MonoGameControls
{
    public class MonoGameViewModel : ViewModel, IMonoGameViewModel
    {
        public MonoGameViewModel()
        {
        }

        public void Dispose()
        {
            Content?.Dispose();
        }

        public IGraphicsDeviceService GraphicsDeviceService { get; set; }
        protected GraphicsDevice GraphicsDevice => GraphicsDeviceService?.GraphicsDevice;
        protected MonoGameServiceProvider Services { get; private set; }
        protected ContentManager Content { get; set; }

        protected IGameTimeService GameTimeService { get; set; }

        public virtual void Initialize(IGameTimeService gameTimeService)
        {
            Services = new MonoGameServiceProvider();
            Services.AddService(GraphicsDeviceService);
            Content = new ContentManager(Services) { RootDirectory = "Content" };
            GameTimeService = gameTimeService;
        }

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update() { }
        public virtual void Draw() { }
        public virtual void OnActivated(object sender, EventArgs args) { }
        public virtual void OnDeactivated(object sender, EventArgs args) { }
        public virtual void OnExiting(object sender, EventArgs args) { }
        public virtual void SizeChanged(object sender, SizeChangedEventArgs args) { }
    }
}
