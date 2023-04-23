using FrigidRogue.MonoGame.Core.View;
using WaveFunctionCollapse.Views;

namespace WaveFunctionCollapse.Screens;

public class WaveFunctionCollapseScreen : Screen
{
    private readonly WaveFunctionCollapseView _waveFunctionCollapseView;

    public WaveFunctionCollapseScreen(WaveFunctionCollapseView waveFunctionCollapseView) : base(waveFunctionCollapseView)
    {
        _waveFunctionCollapseView = waveFunctionCollapseView;
    }

    public void ShowScreen()
    {
        _waveFunctionCollapseView.Initialise();

        Show();
    }
}
