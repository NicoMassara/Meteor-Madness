using System;
using _Main.Scripts.Shield.Rotation;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace _Main.Scripts.Shield
{
    public class ShieldSpeeder
    {
        private readonly IShieldMovement _movement;
        private readonly ShieldSpeederData _data;

        private float _speedRatio; // 0..1
        private State _state = State.Idle;
        
        public event Action OnSpeedIncreased;
        public event Action OnSpeedDecreased;
        
        public event Action OnStartDecreasing;

        private enum State
        {
            Idle,
            Increasing,
            Decreasing
        }

        [Serializable]
        public class ShieldSpeederData
        {
            [SerializeField] private float speedMultiplier = 5f;

            [SerializeField] private float timeToIncrease = 0.5f;
            [SerializeField] private float timeToDecrease = 0.5f;

            [SerializeField] private float timeToFinishIncreasing = 1f;
            [SerializeField] private float startDecreaseDelay = 1f;

            public float SpeedMultiplier => speedMultiplier;
            public float TimeToIncrease => timeToIncrease;
            public float TimeToDecrease => timeToDecrease;
            public float TimeToFinishIncreasing => timeToFinishIncreasing;
            public float StartDecreaseDelay => startDecreaseDelay;
        }
        
        public ShieldSpeeder(IShieldMovement movement, ShieldSpeederData data)
        {
            _movement = movement;
            _data = data;
            
            _decreaseDelayTimer = _data.StartDecreaseDelay;
        }

        public bool IsActive => _state != State.Idle;

        public void IncreaseSpeed()
        {
            _state = State.Increasing;
        }

        public void DecreaseSpeed()
        {
            _state = State.Decreasing;
        }

        public void UpdateSpeed(float deltaTime)
        {
            if(_state == State.Idle) return;
            
            switch (_state)
            {
                case State.Increasing: HandleIncreasing(deltaTime); break;
                case State.Decreasing: HandleDecreasing(deltaTime); break;
            }
            
        }

        private void HandleIncreasing(float deltaTime)
        {
            _speedRatio += deltaTime / _data.TimeToIncrease;
            _speedRatio = Mathf.Clamp01(_speedRatio);
            _movement.SetSpeedMultiplier(_speedRatio * _data.SpeedMultiplier);
            _movement.SetDirection(1);
            
            if (_speedRatio >= 1)
            {
                TimerManager.Add(new TimerData(_data.TimeToFinishIncreasing, () =>
                {
                    _state = State.Idle;
                    OnSpeedIncreased?.Invoke();
                }));
            }
        }
        
        private float _decreaseDelayTimer;
        
        private void HandleDecreasing(float deltaTime)
        {
            if (_decreaseDelayTimer > 0)
            {
                _decreaseDelayTimer -= deltaTime;
                return;
            }

            _speedRatio -= deltaTime / _data.TimeToDecrease;
            _speedRatio = Mathf.Clamp01(_speedRatio);

            var finalRatio = Mathf.Lerp(1, _data.SpeedMultiplier, _speedRatio * _data.SpeedMultiplier);
            
            Debug.Log($"Final Ratio: {finalRatio}, Speed Ratio: {_speedRatio}");
            
            _movement.SetSpeedMultiplier(finalRatio);
            _movement.SetDirection(1);

            if (_speedRatio <= 0)
            {
                _state = State.Idle;
                _decreaseDelayTimer = _data.StartDecreaseDelay;
                _movement.SetSpeedMultiplier(1);
                OnSpeedDecreased?.Invoke();
            }
        }

        public void Reset()
        {
            _speedRatio = 0;
            _state = State.Idle;
            _movement.SetDirection(0);
        }
    }

}