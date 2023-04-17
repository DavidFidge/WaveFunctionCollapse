using System.Diagnostics;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.UserInterface;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View.Interfaces;

using InputHandlers.Keyboard;
using InputHandlers.Mouse;

using Microsoft.Extensions.Configuration;
using WaveFunctionCollapse.Input;
using WaveFunctionCollapse.ViewModels;
using WaveFunctionCollapse.Views;

namespace WaveFunctionCollapse.Installers;

public class GameInstaller : IWindsorInstaller
{
    [Conditional("DEBUG")]
    private void SetDebugEnvironment(ref string environment)
    {
        environment = "Development";
    }
        
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        var environment = "Production";
            
        SetDebugEnvironment(ref environment);
            
        var configuration =  new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();
            
        container.Register(Component.For<IConfiguration>().Instance(configuration));

        RegisterWaveFunctionCollapseView(container, store);
        RegisterKeyboardHandlers(container);
        RegisterMouseHandlers(container);

        container.Register(

            Component.For<IGame>()
                .ImplementedBy<WaveFunctionCollapseGame>(),

            Classes.FromAssembly(Assembly.GetExecutingAssembly())
                .BasedOn<IScreen>(),

            Classes.FromAssemblyContaining<IGameCamera>()
                .BasedOn<IGameCamera>()
                .WithServiceDefaultInterfaces(),

            Component.For<IActionMapStore>()
                .ImplementedBy<DefaultActionMapStore>(),

            Classes.FromAssembly(Assembly.GetExecutingAssembly())
                .BasedOn<BaseGameActionCommand>()
                .LifestyleTransient(),

            Component.For<ICameraMovement>()
                .ImplementedBy<CameraMovement>()
        );
    }

    private void RegisterMouseHandlers(IWindsorContainer container)
    {
        container.Register(
            Classes.FromAssembly(Assembly.GetExecutingAssembly())
                .BasedOn<IMouseHandler>() // Only covers dependencies asking for IMouseHandler, does not cover if they ask for specific class type e.g. see RegisterGameView
                .ConfigureFor<NullMouseHandler>(c => c.IsDefault())
                .WithServiceDefaultInterfaces()
        );
    }

    private void RegisterKeyboardHandlers(IWindsorContainer container)
    {
            
        container.Register(
            Classes.FromAssembly(Assembly.GetExecutingAssembly())
                .BasedOn<IKeyboardHandler>() // Only covers dependencies asking for IKeyboardHandler, does not cover if they ask for specific class type e.g. see RegisterGameView
                .Unless(s => typeof(GlobalKeyboardHandler).IsAssignableFrom(s))
                .ConfigureFor<NullKeyboardHandler>(c => c.IsDefault())
                .WithServiceDefaultInterfaces(),

            Component.For<GlobalKeyboardHandler>()
        );
    }

    private void RegisterWaveFunctionCollapseView(IWindsorContainer container, IConfigurationStore store)
    {
        container.Register(
            Component.For<WaveFunctionCollapseView>()
                .DependsOn(Dependency.OnComponent<IKeyboardHandler, WaveFunctionCollapseViewKeyboardHandler>())
                .DependsOn(Dependency.OnComponent<IMouseHandler, WaveFunctionCollapseMouseHandler>()),

            Component.For<WaveFunctionCollapseViewModel>()
        );
    }
}