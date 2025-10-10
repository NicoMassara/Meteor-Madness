using _Main.Scripts.Interfaces;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.Shaker
{
    public class ShakerController
    {
        private float _shakeTimer;
        private readonly Vector3 _startPosition;
        private IShakeData _shakeData;
        private readonly Transform _transform;
        private float _multiplier = 1f;
        
        public bool IsShaking => _shakeTimer > 0f;

        public ShakerController(Transform transform)
        {
            _transform = transform;
            _startPosition = _transform.position;
        }
        
        public ShakerController(Transform transform, ShakeDataSo data)
        {
            _transform = transform;
            _startPosition = _transform.localPosition;
            SetShakeData(data);
        }

        public void SetShakeData(IShakeData data)
        {
            _shakeData = data;
        }

        public void HandleShake(float deltaTime)
        {
            if(_shakeData == null) return;
            
            if (_shakeData.DoesLoop)
            {
                _shakeTimer += deltaTime;
                _transform.localPosition = GetShakeOffset(_shakeTimer) + _startPosition;
            }
            else if (_shakeTimer > 0)
            {
                _shakeTimer -= deltaTime;
                _transform.localPosition = GetShakeOffset(_shakeTimer) + _startPosition;
                
                if (_shakeTimer <= 0)
                {
                    _transform.localPosition = _startPosition;
                }
            }
        }

        public void StartShake()
        {
            if(_shakeData == null) return;
            
            _shakeTimer = _shakeData.ShakeTime;
        }
        
        public void SetMultiplier(float multiplier)
        {
            _multiplier = multiplier;
        }

        private Vector3 GetShakeOffset(float time)
        {
            float angle = ((_shakeData.ShakeIntensity * _multiplier) * Mathf.PI * 2f) * time;
            float offsetX = Mathf.Sin(angle) * (_shakeData.XShakeMagnitude * _multiplier);
            float offsetY = Mathf.Cos(angle) * (_shakeData.YShakeMagnitude * _multiplier);
            return new Vector3(offsetX, offsetY, 0f);
        }
    }
}