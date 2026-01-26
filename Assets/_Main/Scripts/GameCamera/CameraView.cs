using System;
using _Main.Scripts.Contracts.Interfaces;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Common.Shaker;
using MeteorMadness.GlobalValues.Tools.Observer;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.GameCamera
{
    public class CameraView : ManagedBehavior, ILateUpdatable, IObserver
    {
        [Header("Components")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject grayScaleStencil;
        
        private ComponentShaker _shakerController;
        private ActionManager.GeneratedId _moveActionId;
        private ActionManager.GeneratedId _zoomActionId;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Camera;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;
        
        
        private void Start()
        {
            _shakerController = new ComponentShaker(mainCamera.transform);
            HandleDisableGrayscale();
        }
        
        public void ExecuteLateUpdate(float deltaTime)
        {
            if (_shakerController.IsShaking)
            {
                _shakerController.HandleShake(deltaTime);
                
                if (_shakerController.IsShaking == false)
                {
                    CameraEventCaller.NotifyShakeFinished();
                }
            }
        }
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                // === Shake === //
                case CameraObserverMessage.Shake:
                    HandleShake((IShakeData)args[0]);
                    break;
                
                // === Move === // 
                case CameraObserverMessage.Move:
                    HandleMove((IMovementData)args[0],(float)args[1]);
                    break;
                
                // === Zoom === // 
                case CameraObserverMessage.Zoom:
                    HandleZoom((IZoomData)args[0],(float)args[1]);
                    break;
                
                // === Grayscale === //
                case CameraObserverMessage.EnableGrayscale:
                    HandleEnableGrayscale();
                    break;
                case CameraObserverMessage.DisableGrayscale:
                    HandleDisableGrayscale();
                    break;
            }
        }

        #region Zoom

        private class ZoomAction : IQueueAction
        {
            private readonly Camera _gameCamera;
            private readonly IZoomData _zoomData;
            private readonly float _targetTime;
            
            private float _startZoom;
            private float _elapsedTime;
            
            public ActionStatus CurrentStatus { get; private set; }

            public ZoomAction(Camera gameCamera, IZoomData zoomData, float targetTime)
            {
                _gameCamera = gameCamera;
                _zoomData = zoomData;
                _targetTime = targetTime;
            }

            public void OnStart()
            {
                _startZoom = _gameCamera.orthographicSize;
                
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                _elapsedTime += deltaTime;
                
                float ratio = Mathf.Clamp01(_elapsedTime / _targetTime);
                var curveValue = _zoomData.Curve.Evaluate(ratio);
                float currentValue = Mathf.LerpUnclamped(_startZoom, _zoomData.Value, curveValue);

                if (ratio >= 1f)
                {
                    currentValue = _zoomData.Value;
                    CurrentStatus = ActionStatus.Success;
                }
                
                _gameCamera.orthographicSize = currentValue;

                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                _gameCamera.orthographicSize = _zoomData.Value;
                CurrentStatus = ActionStatus.Failure;
            }

            public IQueueAction Copy() => null;
        }

        private void HandleZoom(IZoomData zoomData, float targetTime)
        {
            if (_zoomActionId is { IsActive: true })
            {
                ActionManager.Remove(_zoomActionId);
            }

            var zoomAction = new ZoomAction(mainCamera, zoomData, targetTime);
            var actions = ActionBuilder.Start()
                .Do(zoomAction)
                .Then(new InstantAction(CameraEventCaller.NotifyZoomFinished))
                .Build();
            
            _zoomActionId = ActionManager.Add(actions, ActionManager.UpdateType.Late);
        }

        #endregion

        #region Move

        private class MoveAction : IQueueAction
        {
            private readonly Camera _gameCamera;
            private readonly IMovementData _movementData;
            private readonly float _targetTime;

            private Vector3 _startPos;
            private float _elapsedTime;

            public ActionStatus CurrentStatus { get; private set; }

            public MoveAction(Camera gameCamera, IMovementData movementData, float targetTime)
            {
                _gameCamera = gameCamera;
                _movementData = movementData;
                _targetTime = targetTime;
            }

            public void OnStart()
            {
                _startPos = _gameCamera.transform.position;
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                _elapsedTime += deltaTime;
                
                float ratio = Mathf.Clamp01(_elapsedTime / _targetTime);
                var curveValue = _movementData.Curve.Evaluate(ratio);
                var currentPosition = Vector3.LerpUnclamped(_startPos, _movementData.Position, curveValue);

                if (ratio >= 1)
                {
                    currentPosition = _movementData.Position;
                    
                    CurrentStatus = ActionStatus.Success;
                }
                
                _gameCamera.transform.position = currentPosition;

                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                _gameCamera.transform.position = _movementData.Position;
                CurrentStatus = ActionStatus.Failure;
            }

            public IQueueAction Copy() => null;
        }
        
        private void HandleMove(IMovementData movementData, float targetTime)
        {
            if (_moveActionId is { IsActive: true })
            {
                ActionManager.Remove(_moveActionId);
            }
            
            var zoomAction = new MoveAction(mainCamera, movementData,targetTime);
            var actions = ActionBuilder.Start()
                .Do(zoomAction)
                .Then(new InstantAction(CameraEventCaller.NotifyLookFinished))
                .Build();
            
            _moveActionId = ActionManager.Add(actions, ActionManager.UpdateType.Late);
        }

        #endregion

        #region Grayscake

        private void HandleEnableGrayscale() 
            => grayScaleStencil.SetActive(true);

        private void HandleDisableGrayscale() 
            => grayScaleStencil.SetActive(false);

        #endregion
        
        private void HandleShake(IShakeData shakeData)
        {
            _shakerController.SetShakeData(shakeData);
            _shakerController.StartShake();
        }

        public InitialCameraData GetCameraData()
        {
            return new InitialCameraData
            {
                Position = mainCamera.transform.position,
                Zoom = mainCamera.orthographicSize
            };
        }
    }
}