using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Observer;
using _Main.Scripts.Shaker;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.MyCamera
{
    public class CameraView : ManagedBehavior, ILateUpdatable, IObserver
    {
        [Header("Components")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera dummyCamera;
        [Range(0, 0.5f)] 
        [SerializeField] 
        private float horizontalLookOffset = 0.3f;
        [Range(0, 0.5f)] 
        [SerializeField] 
        private float verticalLookOffset = 0.3f;
        
        private ShakerController _shakerController;
        private const float ZoomOutSize = 11;
        private const float ZoomInSize = 6;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Camera;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;
        
        public event Action OnActionEnd;
        
        private void Start()
        {
            _shakerController = new ShakerController(mainCamera.transform);
        }
        
        public void ExecuteLateUpdate(float deltaTime)
        {
            if (_shakerController.IsShaking)
            {
                _shakerController.HandleShake(deltaTime);
                
                if (_shakerController.IsShaking == false)
                {
                    TriggerEndAction();
                    CameraEventCaller.NotifyShakeFinished();
                }
            }
        }
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case CameraObserverMessage.Shake:
                    HandleShake((IShakeData)args[0]);
                    break;
                //
                case CameraObserverMessage.ZoomIn:
                    HandleZoom(CameraZoomPosition.ZoomIn, (float)args[0]);
                    break;
                case CameraObserverMessage.ZoomOut:
                    HandleZoom(CameraZoomPosition.ZoomOut, (float)args[0]);
                    break;
                //
                case CameraObserverMessage.LookCenter:
                    HandleLook(CameraLookPosition.Center, (float)args[0]);
                    break;
                case CameraObserverMessage.LookRight:
                    HandleLook(CameraLookPosition.Right, (float)args[0]);
                    break;
                case CameraObserverMessage.LookLeft:
                    HandleLook(CameraLookPosition.Left, (float)args[0]);
                    break;
                case CameraObserverMessage.LookTop:
                    HandleLook(CameraLookPosition.Top, (float)args[0]);
                    break;
                case CameraObserverMessage.LookBottom:
                    HandleLook(CameraLookPosition.Bottom, (float)args[0]);
                    break;
            }
        }

        private void HandleShake(IShakeData shakeData)
        {
            _shakerController.SetShakeData(shakeData);
            _shakerController.StartShake();
        }
        
        private void HandleZoom(CameraZoomPosition zoomPosition, float timeToZoom)
        {
            switch (zoomPosition)
            {
                case CameraZoomPosition.ZoomOut:
                    ZoomOut(timeToZoom);
                    break;
                case CameraZoomPosition.ZoomIn:
                    ZoomIn(timeToZoom);
                    break;
            }
        }

        private void HandleLook(CameraLookPosition position, float timeToLook)
        {
            switch (position)
            {
                case CameraLookPosition.Center:
                    LookCenter(timeToLook);
                    break;
                case CameraLookPosition.Right:
                    LookRight(timeToLook);
                    break;
                case CameraLookPosition.Left:
                    LookLeft(timeToLook);
                    break;
                case CameraLookPosition.Top:
                    LookUp(timeToLook);
                    break;
                case CameraLookPosition.Bottom:
                    LookDown(timeToLook);
                    break;
            }
        }
        
        #region Look Right/Left/Center
        
        private class LookAction : IQueueAction
        {
            private readonly Vector3 _targetPosition;
            private readonly float _targetTime;
            private readonly Camera _mainCamera;
            private Vector3 _startPosition;
            private float _elapsedTime;
            private Vector3 _cameraPosition;

            public LookAction(Vector3 targetPosition, float targetTime, Camera mainCamera)
            {
                _targetPosition = targetPosition;
                _targetTime = targetTime;
                _mainCamera = mainCamera;
            }

            public ActionStatus CurrentStatus { get; private set; }
            
            public void OnStart()
            {
                _cameraPosition = _mainCamera.transform.position;
                _startPosition = _cameraPosition;
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                _elapsedTime += deltaTime;
                
                float ratio = Mathf.Clamp01(_elapsedTime / _targetTime);
                float easedRatio = Mathf.SmoothStep(0f, 1f, ratio);
                _cameraPosition = Vector3.Lerp(_startPosition, _targetPosition, easedRatio);

                if (ratio >= 1)
                {
                    _cameraPosition = _targetPosition;
                    
                    CurrentStatus = ActionStatus.Success;
                }
                
                _mainCamera.transform.position = _cameraPosition;

                return CurrentStatus;
            }

            public void OnInterrupt() {}

            public IQueueAction Copy() => null;
        }

        private void LookRight(float timeToMove)
        {
            var value  = GetWorldPointFromScreen(Screen.width * (1-horizontalLookOffset),Screen.height * 0.5f);
            
            var actions = ActionBuilder.Start().
                Do(new LookAction(value,
                    timeToMove, mainCamera))
                .Then(new InstantAction(TriggerEndAction))
                .Then(new InstantAction(CameraEventCaller.NotifyLookFinished))
                .Build();

            ActionManager.Add(actions, ActionManager.UpdateType.Late);
        }
        
        private void LookCenter(float timeToMove)
        {
            var value = GetWorldPointFromScreen(Screen.width * 0.5f, Screen.height * 0.5f);
            
            var actions = ActionBuilder.Start().
                Do(new LookAction(value,
                    timeToMove, mainCamera))
                .Then(new InstantAction(TriggerEndAction))
                .Then(new InstantAction(CameraEventCaller.NotifyLookFinished))
                .Build();
            
            ActionManager.Add(actions, ActionManager.UpdateType.Late);
        }
        
        private void LookLeft(float timeToMove)
        {
            var value = GetWorldPointFromScreen(Screen.width * horizontalLookOffset,Screen.height * 0.5f); 
            
            var actions = ActionBuilder.Start().
                Do(new LookAction(value,
                    timeToMove, mainCamera))
                .Then(new InstantAction(TriggerEndAction))
                .Then(new InstantAction(CameraEventCaller.NotifyLookFinished))
                .Build();
            
            ActionManager.Add(actions, ActionManager.UpdateType.Late);
        }
        
        private void LookUp(float timeToMove)
        {
            var value = GetWorldPointFromScreen(Screen.width * 0.5f, Screen.height * (1-verticalLookOffset));
            
            var actions = ActionBuilder.Start().
                Do(new LookAction(value,
                    timeToMove, mainCamera))
                .Then(new InstantAction(TriggerEndAction))
                .Then(new InstantAction(CameraEventCaller.NotifyLookFinished))
                .Build();
            
            ActionManager.Add(actions, ActionManager.UpdateType.Late);
        }
        
        private void LookDown(float timeToMove)
        {
            var value = GetWorldPointFromScreen(Screen.width * 0.5f, Screen.height * verticalLookOffset);
            
            var actions = ActionBuilder.Start().
                Do(new LookAction(value,
                    timeToMove, mainCamera))
                .Then(new InstantAction(TriggerEndAction))
                .Then(new InstantAction(CameraEventCaller.NotifyLookFinished))
                .Build();
            
            ActionManager.Add(actions, ActionManager.UpdateType.Late);
        }
        
        private Vector3 GetWorldPointFromScreen(float screenX, float screenY)
        {
            Vector3 screenPoint = new Vector3(screenX, screenY, dummyCamera.nearClipPlane);
            return dummyCamera.ScreenToWorldPoint(screenPoint);
        }
        
        #endregion
        
        #region Zoom

        private class ZoomAction : IQueueAction
        {
            private readonly float _startSize;
            private readonly float _targetSize;
            private readonly float _zoomTime;
            private readonly Camera _mainCamera;
            private float _elapsedTime;
            
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;

            public ZoomAction(float startSize, float targetSize, float zoomTime, Camera mainCamera)
            {
                _startSize = startSize;
                _targetSize = targetSize;
                _zoomTime = zoomTime;
                _mainCamera = mainCamera;
            }

            public void OnStart()
            {
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                _elapsedTime += deltaTime;
                
                float ratio = Mathf.Clamp01(_elapsedTime / _zoomTime);
                float currentValue = Mathf.Lerp(_startSize, _targetSize, Mathf.SmoothStep(0f, 1f, ratio));

                if (ratio >= 1)
                {
                    currentValue = _targetSize;
                    CurrentStatus = ActionStatus.Success;
                }
                
                _mainCamera.orthographicSize = currentValue;

                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                Debug.Log("Interrupted");
                _mainCamera.orthographicSize = _targetSize;
                CurrentStatus = ActionStatus.Failure;
            }

            public IQueueAction Copy()
            {
                return new ZoomAction(_startSize, _targetSize, _zoomTime, _mainCamera);
            }
        }
        

        private void ZoomIn(float timeToZoom)
        {
            var actions = ActionBuilder.Start().
                Do(new ZoomAction(GetCameraSize(), 
                    ZoomInSize, timeToZoom, 
                    mainCamera))
                .Then(new InstantAction(CameraEventCaller.NotifyZoomFinished))
                .Then(new InstantAction(TriggerEndAction))
                .Then(new WaitFramesAction(1))
                .Build();
            
            ActionManager.Add(actions, ActionManager.UpdateType.Late);
        }

        private void ZoomOut(float timeToZoom)
        {
            var actions = ActionBuilder.Start().
                Do(new ZoomAction(GetCameraSize(), 
                    ZoomOutSize, timeToZoom, 
                    mainCamera))
                .Then(new InstantAction(CameraEventCaller.NotifyZoomFinished))
                .Then(new InstantAction(TriggerEndAction))
                .Then(new WaitFramesAction(1))
                .Build();
            
            ActionManager.Add(actions, ActionManager.UpdateType.Late);
        }

        private float GetCameraSize()
        {
            return mainCamera.orthographicSize;
        }

        #endregion

        private void TriggerEndAction()
        {
            OnActionEnd?.Invoke();
        }
    }
}