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
            public void Initialize();
            public void TransitionToIdle();
            public void TryShake(IShakeData shakeData);
            public void TryTransport(ICameraTransportData transportData);
            public void SetActiveGrayscale(bool isActive);
        }
        
        private interface IController
        {
            
        }

        #endregion
        
        private CameraMotor _motor;

        public CameraController(CameraMotor motor)
        {
            _motor = motor;
        }

        #region ICameraController

        public void Initialize()
        {
            
        }

        public void TransitionToIdle()
        {
            
        }

        public void TryShake(IShakeData shakeData) 
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