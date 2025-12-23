using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Observer;

namespace _Main.Scripts.MyCamera
{
    public class CameraMotor : ObservableComponent
    {
        private float _timeToZoom;
        private float _timeToLook;
        private IShakeData _shakeData;
        
        #region Shake Actions

        public void TryShake(IShakeData shakeData)
        {                
            _shakeData = shakeData;
        }
        
        public void ExecuteShake()
        {
            NotifyAll(CameraObserverMessage.Shake,_shakeData);
        }

        #endregion

        #region Zoom Actions

        public void TryZoom(float timeToZoom) => _timeToZoom = timeToZoom;

        private void ExecuteZoom(CameraZoomPosition zoomPosition)
        {
            ulong observerMessage = zoomPosition switch
            {
                CameraZoomPosition.ZoomOut => CameraObserverMessage.ZoomOut,
                CameraZoomPosition.ZoomIn => CameraObserverMessage.ZoomIn,
                _ => throw new ArgumentOutOfRangeException()
            };

            NotifyAll(observerMessage,_timeToZoom);
        }

        public void ExecuteZoomIn() => ExecuteZoom(CameraZoomPosition.ZoomIn);

        public void ExecuteZoomOut() => ExecuteZoom(CameraZoomPosition.ZoomOut);

        #endregion

        #region Look Actions
        
        private void ExecuteLook(CameraLookPosition position)
        {
            ulong observerMessage = position switch
            {
                CameraLookPosition.Center => CameraObserverMessage.LookCenter,
                CameraLookPosition.Right => CameraObserverMessage.LookRight,
                CameraLookPosition.Left => CameraObserverMessage.LookLeft,
                CameraLookPosition.Top => CameraObserverMessage.LookTop,
                CameraLookPosition.Bottom => CameraObserverMessage.LookBottom,
                _ => 0
            };
            
            NotifyAll(observerMessage,_timeToLook);
        }


        public void TryLook(float timeToLook) => _timeToLook = timeToLook;
        public void LookCenter() => ExecuteLook(CameraLookPosition.Center);
        public void LookRight() => ExecuteLook(CameraLookPosition.Right);
        public void LookLeft() => ExecuteLook(CameraLookPosition.Left);
        public void LookAtTop() => ExecuteLook(CameraLookPosition.Top);
        public void LookAtBottom() => ExecuteLook(CameraLookPosition.Bottom);

        #endregion

        #region GrayScale
        
        public void EnableGrayscale() => NotifyAll(CameraObserverMessage.EnableGrayscale);
        public void DisableGrayscale() => NotifyAll(CameraObserverMessage.DisableGrayscale);

        #endregion
    }
}