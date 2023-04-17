using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Messages;

using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Input;

public class CameraMovement : BaseComponent, ICameraMovement
{
    private bool _rotationsEnabled = false;
    private bool _panEnabled = true;
    private bool _zoomEnabled = true;
        
    public void MoveCamera(Keys[] keysDown)
    {
        var cameraMovementFlags = CameraMovementType.None;

        if (_panEnabled)
        {
            if (keysDown.Contains(Keys.Delete))
                cameraMovementFlags |= CameraMovementType.PanLeft;

            if (keysDown.Contains(Keys.PageDown))
                cameraMovementFlags |= CameraMovementType.PanRight;

            if (keysDown.Contains(Keys.Home))
                cameraMovementFlags |= CameraMovementType.PanUp;

            if (keysDown.Contains(Keys.End))
                cameraMovementFlags |= CameraMovementType.PanDown;
        }

        if (_rotationsEnabled)
        {
            if (keysDown.Contains(Keys.Insert))
                cameraMovementFlags |= CameraMovementType.RotateLeft;

            if (keysDown.Contains(Keys.PageUp))
                cameraMovementFlags |= CameraMovementType.RotateRight;

            if (keysDown.Contains(Keys.OemSemicolon))
                cameraMovementFlags |= CameraMovementType.RotateUp;

            if (keysDown.Contains(Keys.OemQuotes))
                cameraMovementFlags |= CameraMovementType.RotateDown;
        }

        if (_zoomEnabled)
        {
            if (keysDown.Contains(Keys.OemPlus))
                cameraMovementFlags |= CameraMovementType.Forward;
                        
            if (keysDown.Contains(Keys.OemMinus))
                cameraMovementFlags |= CameraMovementType.Backward;
        }
            
        if (cameraMovementFlags != CameraMovementType.None)
            Mediator.Send(new MoveViewContinousRequest(cameraMovementFlags));
    }

    public void ZoomCamera(float magnitude)
    {
        Mediator.Send(new ZoomViewRequest(magnitude));
    }
}