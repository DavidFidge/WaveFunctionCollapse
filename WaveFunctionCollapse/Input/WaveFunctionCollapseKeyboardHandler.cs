using InputHandlers.Keyboard;
using Microsoft.Xna.Framework.Input;
using WaveFunctionCollapse.Messages;

namespace WaveFunctionCollapse.Input;

public class WaveFunctionCollapseViewKeyboardHandler : BaseGameViewKeyboardHandler
{
    public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
    {
        base.HandleKeyboardKeyDown(keysDown, keyInFocus, keyboardModifier);

        if (ActionMap.ActionIs<NextStepRequest>(keyInFocus, keyboardModifier))
            Mediator.Send(new NextStepRequest());

        if (ActionMap.ActionIs<NewMapRequest>(keyInFocus, keyboardModifier))
            Mediator.Send(new NewMapRequest());

        if (ActionMap.ActionIs<PlayUntilCompleteRequest>(keyInFocus, keyboardModifier))
            Mediator.Send(new PlayUntilCompleteRequest());

        if (ActionMap.ActionIs<PlayContinuouslyRequest>(keyInFocus, keyboardModifier))
            Mediator.Send(new PlayContinuouslyRequest());
    }

    public override void HandleKeyboardKeyRepeat(Keys repeatingKey, KeyboardModifier keyboardModifier)
    {
        base.HandleKeyboardKeyRepeat(repeatingKey, keyboardModifier);

        if (ActionMap.ActionIs<NextStepRequest>(repeatingKey, keyboardModifier))
            Mediator.Send(new NextStepRequest());
    }
}