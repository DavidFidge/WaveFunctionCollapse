using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;

using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Messages;

[ActionMap(Name = "Play Until Complete", DefaultKey = Keys.C)]
public class PlayUntilCompleteRequest : IRequest
{
}