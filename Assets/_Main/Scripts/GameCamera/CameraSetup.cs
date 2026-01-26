using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Managers;
using UnityEngine;

namespace _Main.Scripts.GameCamera
{
    [RequireComponent(typeof(CameraView))]
    public class CameraSetup : MonoBehaviour
    {
        private CameraView _view;
        private CameraController.ICameraController _controller;
        private CameraMotor _motor;

        private void Awake()
        {
            _view = GetComponent<CameraView>();
            _motor = new CameraMotor();
            _controller = new CameraController(_motor);
            _motor.Subscribe(_view);
            
            _controller.Initialize();
            _controller.TransitionToIdle();
            
            SetEventBus();
        }

        private void Start()
        {
            
        }
        
        #region Event Bus

        private void SetEventBus()
        {
            CameraEventSubscriber.Shake(EventBus_Camera_StartShake);
            CameraEventSubscriber.Transport(EventBus_Camera_Transport);
            CameraEventSubscriber.EnableGrayscale(EventBus_Camera_Grayscale_Enable);
            CameraEventSubscriber.DisableGrayscale(EventBus_Camera_Grayscale_Disable);
        }

        private void EventBus_Camera_Transport(CameraEvents.Transport input)
        {
            _controller.TryTransport(input.Data);
        }
        
        private void EventBus_Camera_StartShake(CameraEvents.Shake input)
        {
            _controller.TryShake(input.ShakeData);
        }

        #region GrayScale

        private void EventBus_Camera_Grayscale_Enable(CameraEvents.GrayscaleEnable input)
        {
            _controller.SetActiveGrayscale(true);
        }
        
        private void EventBus_Camera_Grayscale_Disable(CameraEvents.GrayscaleDisable input)
        {
            _controller.SetActiveGrayscale(false);
        }

        #endregion
        
        #endregion
    }
}