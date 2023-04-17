using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Input;

public interface ICameraMovement
{
    void MoveCamera(Keys[] keysDown);
    void ZoomCamera(float magnitude);
}