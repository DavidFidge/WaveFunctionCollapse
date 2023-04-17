using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Input;

public class GlobalKeyboardHandler : BaseKeyboardHandler
{
    public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
    {
        if (keyInFocus == Keys.Enter && keyboardModifier.IsAltDown())
            Mediator.Send(new ToggleFullScreenRequest());
    }
}