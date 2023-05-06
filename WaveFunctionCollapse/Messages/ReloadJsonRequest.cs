using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;

using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Messages;

[ActionMap(Name = "Reload Json", DefaultKey = Keys.R)]
public class ReloadJsonRequest : IRequest
{
}