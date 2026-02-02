using _Main.Scripts.Common;
using _Main.Scripts.Contracts.Interfaces;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace _Main.Scripts.GameCamera
{
    public class CameraMotor : ObservableComponent
    {
        private float _zoom;
        private Vector2 _position;
        
        public void Initialize(InitialCameraData data)
        {
            _zoom = data.Zoom;
            _position = data.Position;
        }
        
        public void ShakeCamera(ShakeData shakeData)
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
            var targetTime = transportData.Time;

            if (GetDoesZoom(cameraZoom))
            {
                var zoomValue = cameraZoom.Value;
                // If the current zoom is greater than the input zoomValue then
                // is zooming in
                var doesZoomIn = _zoom > zoomValue;
                
                _zoom = zoomValue;
                NotifyAll(CameraObserverMessage.Zoom,cameraZoom,targetTime,doesZoomIn);
            }
            else
            {
                //Debug.Log("Same Zoom");
            }
            
            var cameraMovement = transportData.CameraMovementData;

            if (GetDoesMove(cameraMovement))
            {
                _position = cameraMovement.Position;
                NotifyAll(CameraObserverMessage.Move,cameraMovement,targetTime);
            }
            else
            {
                Debug.Log("Distance is not enough or does not change");
            }
        }

        private bool GetDoesZoom(IZoomData zoomData)
        {
            if (zoomData.DoesChange)
            {
                if(!Mathf.Approximately(_zoom, zoomData.Value))
                    return true;
            }

            return false;
        }

        private bool GetDoesMove(IMovementData movementData)
        {
            if (movementData.DoesChange)
            {
                var distance = Vector2.Distance(movementData.Position, _position);
                
                if (distance > 0)
                {
                    return true;
                }
            }

            return false;   
        }
    }
}