using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;

using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Messages;

[ActionMap(Name = "Next Step", DefaultKey = Keys.Space)]
public class NextStepRequest : IRequest
{
}