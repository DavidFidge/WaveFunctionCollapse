﻿using InputHandlers.Keyboard;
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

        if (ActionMap.ActionIs<MewMapRequest>(keyInFocus, keyboardModifier))
            Mediator.Send(new MewMapRequest());

        if (ActionMap.ActionIs<PlayUntilCompleteRequest>(keyInFocus, keyboardModifier))
            Mediator.Send(new PlayUntilCompleteRequest());
    }

    public override void HandleKeyboardKeyRepeat(Keys repeatingKey, KeyboardModifier keyboardModifier)
    {
        base.HandleKeyboardKeyRepeat(repeatingKey, keyboardModifier);

        if (ActionMap.ActionIs<NextStepRequest>(repeatingKey, keyboardModifier))
            Mediator.Send(new NextStepRequest());
    }
}