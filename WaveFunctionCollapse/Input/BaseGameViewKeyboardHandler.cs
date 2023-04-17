using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Input;

public class BaseGameViewKeyboardHandler : BaseKeyboardHandler
{
    public ICameraMovement CameraMovement { get; set; }
        
    public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
    {
        if (keyInFocus == Keys.F12)
            Environment.Exit(0);

        CameraMovement.MoveCamera(keysDown);
    }

    public override void HandleKeyboardKeysReleased()
    {
        Mediator.Send(new MoveViewContinousRequest(CameraMovementType.None));
    }
}