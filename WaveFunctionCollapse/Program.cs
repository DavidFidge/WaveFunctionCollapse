using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommandLine;
using FrigidRogue.MonoGame.Core.Installers;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.View.Installers;
using WaveFunctionCollapse.Installers;

namespace WaveFunctionCollapse;

public static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunGame);
    }

    static void RunGame(Options options)
    {
        var container = new WindsorContainer();

        container.Install(new CoreInstaller());
        container.Install(new ViewInstaller());
        container.Install(new GameInstaller());
            
        container.Register(
            Component.For<Options>()
                .Instance(options)
        );

        using var game = container.Resolve<IGame>();

        game.Run();
    }
}