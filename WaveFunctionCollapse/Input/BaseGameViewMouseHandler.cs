using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace WaveFunctionCollapse.Input;

    public abstract class BaseGameViewMouseHandler : BaseMouseHandler
    {
        private readonly IStopwatchProvider _stopwatchProvider;
        private double _mouseMoveThrottle = 20;
        private double _lastTotalMilliseconds;
        
        private MouseState? _mouseStateBeforeDrag;
        private int _halvedWindowX;
        private int _halvedWindowY;

        public ICameraMovement CameraMovement { get; set; }

        public BaseGameViewMouseHandler(IStopwatchProvider stopwatchProvider)
        {
            _stopwatchProvider = stopwatchProvider;
            _stopwatchProvider.Start();
        }
        
        public override void HandleRightMouseDragging(MouseState mouseState, MouseState originalMouseState)
        {
            if (_mouseStateBeforeDrag == null)
            {
                _mouseStateBeforeDrag = originalMouseState;

                _halvedWindowX = Game.Window.ClientBounds.Width / 2;
                _halvedWindowY = Game.Window.ClientBounds.Height / 2;
            }
            else
            {
                var xDisplacement = mouseState.X - _halvedWindowX;
                var yDisplacement = mouseState.Y - _halvedWindowY;

                Mediator.Send(new MoveViewRequest(xDisplacement * 0.5f, yDisplacement * 0.5f));
            }

            Mouse.SetPosition(_halvedWindowX, _halvedWindowY);
        }

        public override void HandleRightMouseDragDone(MouseState mouseState, MouseState originalMouseState)
        {
            if (_mouseStateBeforeDrag != null)
                Mouse.SetPosition(originalMouseState.X, originalMouseState.Y);

            _mouseStateBeforeDrag = null;

            base.HandleRightMouseDragDone(mouseState, originalMouseState);
        }

        protected bool CanSendMouseMoveEvent()
        {
            if (_stopwatchProvider.Elapsed.TotalMilliseconds - _lastTotalMilliseconds > _mouseMoveThrottle)
            {
                _lastTotalMilliseconds = _stopwatchProvider.Elapsed.TotalMilliseconds;
                return true;
            }

            return false;
        }

        public override void HandleMouseScrollWheelMove(MouseState mouseState, int difference)
        {
            CameraMovement.ZoomCamera(difference);
            
            base.HandleMouseScrollWheelMove(mouseState, difference);
        }
    }