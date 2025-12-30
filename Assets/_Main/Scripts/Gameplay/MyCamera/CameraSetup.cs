using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Managers;
using UnityEngine;

namespace _Main.Scripts.MyCamera
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
            _view.OnActionEnd += () =>
            {
                _controller.TransitionToIdle();
            };
        }
        
        #region Event Bus

        private void SetEventBus()
        {
            CameraEventSubscriber.Shake(EventBus_Camera_StartShake);
            CameraEventSubscriber.ZoomIn(EventBus_Camera_ZoomIn);
            CameraEventSubscriber.ZoomOut(EventBus_Camera_ZoomOut);
            CameraEventSubscriber.LookCenter(EventBus_Camera_LookCenter);
            CameraEventSubscriber.LookRight(EventBus_Camera_LookRight);
            CameraEventSubscriber.LookLeft(EventBus_Camera_LookLeft);
            CameraEventSubscriber.LookUp(EventBus_Camera_LookUp);
            CameraEventSubscriber.LookDown(EventBus_Camera_LookDown);
            CameraEventSubscriber.EnableGrayscale(EventBus_Camera_Grayscale_Enable);
            CameraEventSubscriber.DisableGrayscale(EventBus_Camera_Grayscale_Disable);
        }

        #region GrayScale

        private void EventBus_Camera_Grayscale_Enable(CameraEvents.GrayscaleEnable input)
        {
            _controller.EnableGrayscale();
        }
        
        private void EventBus_Camera_Grayscale_Disable(CameraEvents.GrayscaleDisable input)
        {
            _controller.DisableGrayscale();
        }

        #endregion
        
        private void EventBus_Camera_ZoomOut(CameraEvents.ZoomOut input)
        {
            _controller.TryZoomOut(input.TimeToZoom);
        }

        private void EventBus_Camera_ZoomIn(CameraEvents.ZoomIn input)
        {
            _controller.TryZoomIn(input.TimeToZoom);
        }

        private void EventBus_Camera_StartShake(CameraEvents.Shake input)
        {
            _controller.TryShake(input.ShakeData);
        }
        
        private void EventBus_Camera_LookCenter(CameraEvents.LookCenter input)
        {
            _controller.TryLookCenter(input.TimeToLook);
        }
        
        private void EventBus_Camera_LookRight(CameraEvents.LookRight input)
        {
            _controller.TryLookRight(input.TimeToLook);
        }
        
        private void EventBus_Camera_LookLeft(CameraEvents.LookLeft input)
        {
            _controller.TryLookLeft(input.TimeToLook);
        }
        
        private void EventBus_Camera_LookUp(CameraEvents.LookUp input)
        {
            _controller.TryLookAtTop(input.TimeToLook);
        }
        
        private void EventBus_Camera_LookDown(CameraEvents.LookDown input)
        {
            _controller.TryLookAtBottom(input.TimeToLook);
        }
        
        #endregion
    }
}