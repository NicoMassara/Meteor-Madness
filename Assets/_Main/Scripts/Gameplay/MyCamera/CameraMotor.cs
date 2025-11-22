using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Observer;

namespace _Main.Scripts.Gameplay.MyCamera
{
    public class CameraMotor : ObservableComponent
    {
        private CameraZoomPosition _currentZoomPosition;
        private CameraLookPosition _currentLookPosition;
        
        public void Shake(IShakeData shakeData)
        {
            NotifyAll(CameraObserverMessage.Shake,shakeData);
        }

        public void Zoom(CameraZoomPosition zoomPosition, float timeToZoom)
        {
            _currentZoomPosition = zoomPosition;

            ulong observerMessage = _currentZoomPosition switch
            {
                CameraZoomPosition.ZoomOut => CameraObserverMessage.ZoomOut,
                CameraZoomPosition.ZoomIn => CameraObserverMessage.ZoomIn,
                _ => throw new ArgumentOutOfRangeException()
            };

            NotifyAll(observerMessage,timeToZoom);
        }

        public void Look(CameraLookPosition position, float timeToLook)
        {
            _currentLookPosition = position;

            ulong observerMessage = _currentLookPosition switch
            {
                CameraLookPosition.Center => CameraObserverMessage.LookCenter,
                CameraLookPosition.Right => CameraObserverMessage.LookRight,
                CameraLookPosition.Left => CameraObserverMessage.LookLeft,
                CameraLookPosition.Top => CameraObserverMessage.LookTop,
                CameraLookPosition.Bottom => CameraObserverMessage.LookBottom,
                _ => 0
            };

            NotifyAll(observerMessage,timeToLook);
        }

        public bool IsUnableToLook(CameraLookPosition position)
        {
            return _currentLookPosition == position;
        }

        public bool IsUnableToZoom(CameraZoomPosition zoomPosition)
        {
            return _currentZoomPosition == zoomPosition;
        }
    }
}