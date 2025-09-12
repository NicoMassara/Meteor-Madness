using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Gameplay
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

        public UpdateManager.UpdateGroup UpdateGroup { get; } = UpdateManager.UpdateGroup.Gameplay;
        

        private void Awake()
        {
            SetEvents();
        }

        private void Start()
        {
            _shakerController = new ShakerController(mainCamera.transform);
            _defaultSize = mainCamera.orthographicSize;
        }
        
        public void ManagedLateUpdate()
        {
            if (_shakerController.IsShaking)
            {
                _shakerController.HandleShake();
            }

            if (_doesChangeSize)
            {
                HandleSizeChange();
            }
        }

        #region Zoom

        private void HandleSizeChange()
        {
            var newSize = mainCamera.orthographicSize;
            newSize =
                Mathf.Lerp(newSize, _targetSize, zoomSpeed * CustomTime.DeltaTime);
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

        private void StartShake(ShakeDataSo shakeData)
        {
            _shakerController.SetShakeData(shakeData);
            _shakerController.StartShake();
        }

        #endregion

        #region Event Bus

        private void SetEvents()
        {
            var eventBus = GameManager.Instance.EventManager;
            
            eventBus.Subscribe<CameraShake>(EventBus_StartShake);
            eventBus.Subscribe<CameraZoomIn>(EventBus_ZoomIn);
            eventBus.Subscribe<CameraZoomOut>(EventBus_ZoomOut);
        }

        private void EventBus_ZoomOut(CameraZoomOut input)
        {
            ZoomOut();
        }

        private void EventBus_ZoomIn(CameraZoomIn input)
        {
            ZoomIn();
        }

        private void EventBus_StartShake(CameraShake input)
        {
            StartShake(input.ShakeData);
        }

        #endregion
    }
}