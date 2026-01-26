using _Main.Scripts.Contracts.Interfaces;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace _Main.Scripts.GameCamera
{
    public class CameraMotor : ObservableComponent
    {
        private ICameraTransportData _transportData;
        
        public void ShakeCamera(IShakeData shakeData)
        {
            NotifyAll(CameraObserverMessage.Shake,shakeData);
        }

        public void SetActiveGrayScale(bool isActive)
        {
            NotifyAll(isActive ? 
                CameraObserverMessage.EnableGrayscale :
                CameraObserverMessage.DisableGrayscale);
        }

        public void TransportCamera(ICameraTransportData transportData)
        {
            var cameraZoom = transportData.CameraZoomData;
            
            if(GetDoesZoom(cameraZoom))
                NotifyAll(CameraObserverMessage.Zoom,cameraZoom);
            
            var cameraMovement = transportData.CameraMovementData;
            
            if(GetDoesMove(cameraMovement))
                NotifyAll(CameraObserverMessage.Move,cameraMovement);
            
            _transportData = transportData;
        }

        private bool GetDoesZoom(IZoomData zoomData)
        {
            if (zoomData.DoesChange)
            {
                if(!Mathf.Approximately(_transportData.CameraZoomData.Value, zoomData.Value))
                    return true;
            }

            return false;
        }

        private bool GetDoesMove(IMovementData movementData)
        {
            if (movementData.DoesChange)
            {
                var distance = Vector2.Distance(
                    movementData.Position, 
                    _transportData.CameraMovementData.Position);
                
                if (distance > 0)
                {
                    return true;
                }
            }

            return false;   
        }
    }
}