﻿using FrigidRogue.MonoGame.Core.UserInterface;
using MediatR;

using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Messages;

[ActionMap(Name = "MapDimensionsChangedRequest")]
public class MapDimensionsChangedRequest : IRequest
{
}