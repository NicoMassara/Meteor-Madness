using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [Range(0,1f)]
        [SerializeField] private float shakeTime;
        [Range(0.1f, 50f)] 
        [SerializeField] private float shakeIntensity = 20;
        [Range(0,.1f)]
        [SerializeField] private float shakeMagnitude = 0.075f;

        private float _shakeTimer;
        private Vector3 _startPosition;

        private void Start()
        {
            _startPosition = mainCamera.transform.position;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartShake();
            }

            if (_shakeTimer > 0)
            {
                HandleShake();

                if (_shakeTimer <= 0)
                {
                    mainCamera.transform.localPosition = _startPosition;
                }
            }
        }

        private void HandleShake()
        {
            _shakeTimer -= Time.deltaTime;
            mainCamera.transform.localPosition = GetShakeOffset(_shakeTimer) + _startPosition;
        }
        
        public Vector3 GetShakeOffset(float time)
        {
            float angle = (shakeIntensity * Mathf.PI * 2f) * time;
            float offsetX = Mathf.Sin(angle) * shakeMagnitude;
            float offsetY = Mathf.Cos(angle) * shakeMagnitude;
            return new Vector3(offsetX, offsetY, 0f);
        }

        public void StartShake()
        {
            _shakeTimer = shakeTime;
        }
    }
}