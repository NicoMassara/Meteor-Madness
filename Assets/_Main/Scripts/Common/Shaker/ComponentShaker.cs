using MeteorMadness.Contracts.Interfaces;
using UnityEngine;

namespace MeteorMadness.Common.Shaker
{
    public class ComponentShaker
    {
        private float _shakeTimer;
        private Vector3 _startPosition;
        private IShakeData _shakeData;
        private readonly Transform _transform;
        private float _multiplier = 1f;
        private bool _isShaking;
        
        public bool IsShaking => _shakeTimer > 0f;

        public ComponentShaker(Transform transform)
        {
            _transform = transform;
            _startPosition = _transform.position;
        }
        
        public ComponentShaker(Transform transform, ShakeDataSo data)
        {
            _transform = transform;
            SetShakeData(data);
        }

        public void SetShakeData(IShakeData data)
        {
            _shakeData = data;

            FinishShake();
            
            _startPosition = _transform.localPosition;
        }

        public void HandleShake(float deltaTime)
        {
            if(_shakeData == null) return;
            if(_isShaking == false) return;
            
            
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
            
            _isShaking = true;
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

        private void FinishShake()
        {
            if (_isShaking)
            {
                _isShaking = false;
                _transform.localPosition = _startPosition;
            }
        }
    }
}