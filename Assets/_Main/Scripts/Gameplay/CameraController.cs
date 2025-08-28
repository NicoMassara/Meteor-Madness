using System;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        private ShakerController _shakerController;
        private float _defaultSize = 10;
        private float _zoomSize = 6;

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
        }

        public void ZoomIn()
        {
            mainCamera.orthographicSize = _zoomSize;
        }

        public void ZoomOut()
        {
            mainCamera.orthographicSize = _defaultSize;
        }

        public void StartShake(ShakeDataSo shakeData)
        {
            _shakerController.SetShakeData(shakeData);
            _shakerController.StartShake();
        }
    }
}