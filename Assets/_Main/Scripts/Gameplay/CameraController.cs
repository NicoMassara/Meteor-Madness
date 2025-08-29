using System;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [Range(0, 50)] 
        [SerializeField] private float zoomSpeed = 10;
        private ShakerController _shakerController;
        private float _defaultSize = 10;
        private float _zoomSize = 6;
        private bool _doesChangeSize = false;
        private float _targetSize;
        

        private void Start()
        {
            _shakerController = new ShakerController(mainCamera.transform);
            _defaultSize = mainCamera.orthographicSize;
            ZoomIn();
        }

        private void Update()
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

        private void HandleSizeChange()
        {
            var newSize = mainCamera.orthographicSize;
            newSize =
                Mathf.Lerp(newSize, _targetSize, zoomSpeed * Time.deltaTime);
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

        public void StartShake(ShakeDataSo shakeData)
        {
            _shakerController.SetShakeData(shakeData);
            _shakerController.StartShake();
        }
    }
}