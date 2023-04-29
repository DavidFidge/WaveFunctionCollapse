using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;

namespace WaveFunctionCollapse.Messages;

[ActionMap(Name = "Change Content")]
public class ChangeContentRequest : IRequest
{
    public string Content { get; set; }
}