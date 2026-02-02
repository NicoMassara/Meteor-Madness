using _Main.Scripts.Common;
using _Main.Scripts.Contracts.Interfaces;
using MeteorMadness.Contracts.Interfaces;

namespace _Main.Scripts.GameCamera
{
    internal class CameraController : 
        CameraController.ICameraController
    {

        #region Interfaces

        internal interface ICameraController
        {
            public void Initialize(InitialCameraData data);
            public void TryShake(ShakeData shakeData);
            public void TryTransport(ICameraTransportData transportData);
            public void SetActiveGrayscale(bool isActive);
        }
        
        private interface IController
        {
            
        }

        #endregion
        
        private readonly CameraMotor _motor;

        public CameraController(CameraMotor motor)
        {
            _motor = motor;
        }

        #region ICameraController

        public void Initialize(InitialCameraData data)
        {
            _motor.Initialize(data);
        }

        public void TryShake(ShakeData shakeData) 
            => _motor.ShakeCamera(shakeData);

        public void TryTransport(ICameraTransportData transportData) 
            => _motor.TransportCamera(transportData);

        public void SetActiveGrayscale(bool isActive) 
            => _motor.SetActiveGrayScale(isActive);

        #endregion
        
        #region IController

        

        #endregion
    }
}