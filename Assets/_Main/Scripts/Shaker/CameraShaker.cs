using System;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Shaker
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        private ShakerController _shakerController;

        private void Start()
        {
            _shakerController = new ShakerController(mainCamera.transform);
        }

        private void Update()
        {
            if (_shakerController.IsShaking)
            {
                _shakerController.HandleShake();
            }
        }

        public void StartShake(ShakeDataSo shakeData)
        {
            _shakerController.SetShakeData(shakeData);
            _shakerController.StartShake();
        }
    }
}