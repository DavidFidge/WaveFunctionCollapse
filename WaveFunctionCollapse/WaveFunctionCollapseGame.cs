using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;

using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.View.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Serilog;
using WaveFunctionCollapse.Input;
using WaveFunctionCollapse.Screens;

namespace WaveFunctionCollapse;

public class WaveFunctionCollapseGame : Game, IGame
{
    private readonly ILogger _logger;
    private readonly IGameProvider _gameProvider;
    private readonly IGameTimeService _gameTimeService;
    private readonly IGameInputService _gameInputService;
    private readonly IUserInterface _userInterface;
    private readonly IGameOptionsStore _gameOptionsStore;
    private readonly IGameCamera _gameCamera;
    private readonly IMediator _mediator;
    private readonly WaveFunctionCollapseScreen _waveFunctionCollapseScreen;
    private readonly IConfiguration _configuration;
    private readonly Options _options;

    private bool _isExiting;

    public CustomGraphicsDeviceManager CustomGraphicsDeviceManager { get; }
    public EffectCollection EffectCollection
    { get; }
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _renderTarget;

    public WaveFunctionCollapseGame(
        ILogger logger,
        IGameProvider gameProvider,
        IGameTimeService gameTimeService,
        IGameInputService gameInputService,
        IUserInterface userInterface,
        IGameOptionsStore gameOptionsStore,
        IGameCamera gameCamera,
        Options options,
        IMediator mediator,
        WaveFunctionCollapseScreen waveFunctionCollapseScreen,
        GlobalKeyboardHandler globalKeyboardHandler,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _gameProvider = gameProvider;
        _gameProvider.Game = this;
        _gameTimeService = gameTimeService;
        _gameInputService = gameInputService;
        _userInterface = userInterface;
        _gameOptionsStore = gameOptionsStore;

        _logger.Debug("Starting game");

        CustomGraphicsDeviceManager = new CustomGraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        _gameCamera = gameCamera;
        _mediator = mediator;
        _waveFunctionCollapseScreen = waveFunctionCollapseScreen;
        _configuration = configuration;
        _options = options;

        EffectCollection = new EffectCollection(_gameProvider);

        Window.AllowUserResizing = true;
        _gameInputService.AddGlobalKeyboardHandler(globalKeyboardHandler);
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
        _gameCamera.Initialise();
        EffectCollection.Initialize();

        _userInterface.Initialize(Content, "mars");

        var renderResolution = _configuration.GetValueOrEnvironmentVariable("RenderResolution");

        _userInterface.RenderResolution = RenderResolution.RenderResolutions
            .FirstOrDefault(r => r.Name == renderResolution) ?? RenderResolution.Default;

        _gameCamera.RenderResolution = RenderResolution.RenderResolutions
            .FirstOrDefault(r => r.Name == renderResolution) ?? RenderResolution.Default;
        _gameCamera.ContinuousMoveSensitivity = 0.5f;
        _gameCamera.ContinuousRotateSensitivity = 0.01f;
        _gameCamera.ZoomSensitivity = 0.01f;

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        InitializeDisplaySettings();

        _waveFunctionCollapseScreen.ShowScreen();

        _renderTarget = new RenderTarget2D(
            _gameProvider.Game.GraphicsDevice,
            _userInterface.RenderResolution.Width,
            _userInterface.RenderResolution.Height,
            false,
            _gameProvider.Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
            _gameProvider.Game.GraphicsDevice.PresentationParameters.DepthStencilFormat,
            0,
            RenderTargetUsage.PreserveContents
        );

        base.Initialize();
    }

    private void InitializeDisplaySettings()
    {
        bool.TryParse(_configuration.GetValueOrEnvironmentVariable("FullScreen"), out var isFullScreen);

        var isVerticalSync = true;
        var isBorderlessWindowed = true;

        var displayDimensions = new DisplayDimension(
            CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
            CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height,
            0);

        int.TryParse(_configuration.GetValueOrEnvironmentVariable("DisplayWidth"), out var displayWidth); 
        int.TryParse(_configuration.GetValueOrEnvironmentVariable("DisplayHeight"), out var displayHeight);
        
        if (displayWidth > 0 && displayHeight > 0)
        {
            displayDimensions = new DisplayDimension(displayWidth, displayHeight, 0);
        }

        var displaySettings = new DisplaySettings(
            displayDimensions,
            isFullScreen,
            isVerticalSync,
            isBorderlessWindowed
        );

        CustomGraphicsDeviceManager.SetDisplayMode(displaySettings);
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
        // Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        if (_isExiting)
            Exit();

        if (!IsActive)
            return;

        _gameTimeService.Update(gameTime);
        _gameInputService.Poll(GraphicsDevice.Viewport.Bounds);
        _userInterface.Update(gameTime);

        base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        if (!IsActive)
            return;
        
        _gameProvider.Game.GraphicsDevice.RestoreGraphicsDeviceAfterSpriteBatchDraw();

        GeonBit.UI.UserInterface.Active.Draw(_spriteBatch);

        _gameProvider.Game.GraphicsDevice.SetRenderTarget(_renderTarget);

        _gameProvider.Game.GraphicsDevice.RestoreGraphicsDeviceAfterSpriteBatchDraw();

        _gameProvider.Game.GraphicsDevice.Clear(Color.Transparent);

        _userInterface.DrawActiveScreen();

        _gameProvider.Game.GraphicsDevice.SetRenderTarget(null);

        _gameProvider.Game.GraphicsDevice.RestoreGraphicsDeviceAfterSpriteBatchDraw();

        DrawRenderTarget(_spriteBatch);

        GeonBit.UI.UserInterface.Active.DrawMainRenderTarget(_spriteBatch);

        base.Draw(gameTime);
    }

    public void DrawRenderTarget(SpriteBatch spriteBatch)
    {
        var viewportWidth = spriteBatch.GraphicsDevice.Viewport.Width;
        var viewportHeight = spriteBatch.GraphicsDevice.Viewport.Height;

        // draw the main render target
        if (_renderTarget != null && !_renderTarget.IsDisposed)
        {
            // draw render target
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, viewportWidth, viewportHeight), Color.White);
            spriteBatch.End();
        }
    }

    public Task<Unit> Handle(QuitToDesktopRequest request, CancellationToken cancellationToken)
    {
        var videoOptions = _gameOptionsStore.GetFromStore<VideoOptions>()?.State;

        _isExiting = true;
        return Unit.Task;
    }
    
    public Task<Unit> Handle(ToggleFullScreenRequest request, CancellationToken cancellationToken)
    {
        CustomGraphicsDeviceManager.ToggleFullScreen();
        return Unit.Task;
    }
}

