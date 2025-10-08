using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Gameplay.MyCamera
{
    public class CameraController : ManagedBehavior, ILateUpdatable
    {
        [Header("Components")]
        [SerializeField] private Camera mainCamera;
        [Range(0, 50)] 
        [SerializeField] private float zoomSpeed = 10;
        private ShakerController _shakerController;
        private float _defaultSize = 10;
        private float _zoomSize = 6;
        private bool _doesChangeSize = false;
        private float _targetSize;

        public UpdateGroup SelfLateUpdateGroup { get; } = UpdateGroup.Camera;
        

        private void Awake()
        {
            SetEventBus();
        }

        private void Start()
        {
            _shakerController = new ShakerController(mainCamera.transform);
            _defaultSize = mainCamera.orthographicSize;
        }
        
        public void ManagedLateUpdate()
        {
            var dt = CustomTime.GetDeltaTimeByChannel(SelfLateUpdateGroup);
            
            if (_shakerController.IsShaking)
            {
                _shakerController.HandleShake(dt);
            }

            if (_doesChangeSize)
            {
                HandleSizeChange(dt);
            }
        }

        #region Zoom

        private void HandleSizeChange(float deltaTime)
        {
            var newSize = mainCamera.orthographicSize;
            newSize =
                Mathf.Lerp(newSize, _targetSize, zoomSpeed * deltaTime);
            Mathf.Clamp(newSize, _zoomSize, _defaultSize);

            if (newSize == _targetSize)
            {
                _doesChangeSize = false;
            }

            mainCamera.orthographicSize = newSize;
        }

        public void ZoomIn()
        {
            _targetSize = _zoomSize;
            _doesChangeSize = true;
        }

        public void ZoomOut()
        {
            _targetSize = _defaultSize;
            _doesChangeSize = true;
        }

        #endregion
        
        #region Shake

        private void StartShake(IShakeData shakeData)
        {
            _shakerController.SetShakeData(shakeData);
            _shakerController.StartShake();
        }

        #endregion

        #region Event Bus

        private void SetEventBus()
        {
            var eventBus = GameManager.Instance.EventManager;
            
            eventBus.Subscribe<CameraEvents.Shake>(EventBus_Camera_StartShake);
            eventBus.Subscribe<CameraEvents.ZoomIn>(EventBus_Camera_ZoomIn);
            eventBus.Subscribe<CameraEvents.ZoomOut>(EventBus_Camera_ZoomOut);
        }

        private void EventBus_Camera_ZoomOut(CameraEvents.ZoomOut input)
        {
            ZoomOut();
        }

        private void EventBus_Camera_ZoomIn(CameraEvents.ZoomIn input)
        {
            ZoomIn();
        }

        private void EventBus_Camera_StartShake(CameraEvents.Shake input)
        {
            StartShake(input.ShakeData);
        }

        #endregion
    }
}