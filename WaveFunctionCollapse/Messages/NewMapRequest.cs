using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;

using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Messages;

[ActionMap(Name = "New Map", DefaultKey = Keys.N)]
public class MewMapRequest : IRequest
{
}