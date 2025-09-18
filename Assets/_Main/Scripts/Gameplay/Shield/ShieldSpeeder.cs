using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldSpeeder
    {
        private readonly ShieldMovement _movement;
        private readonly float _timeToIncrease;
        private readonly float _timeToDecrease;
        private readonly float _decayConstant;
        private float _elapsedTime = 0;
        private float _currentVelocity;

        private const float TargetVelocity = 150f;

        public ShieldSpeeder(ShieldMovement movement, float timeToIncrease, float timeToDecrease, float decayConstant)
        {
            _movement = movement;
            _timeToIncrease = timeToIncrease;
            _timeToDecrease = timeToDecrease;
            _decayConstant = decayConstant;
        }
        
        public void IncreaseSpeed(float deltaTime)
        {
            _elapsedTime += deltaTime;
            var timeRatio = _elapsedTime / _timeToDecrease;
            _currentVelocity = Mathf.Lerp(0, TargetVelocity, timeRatio);
            _movement.Move(_currentVelocity,deltaTime);
            
            if (timeRatio >= 1)
            {
                _currentVelocity = TargetVelocity;
            }
        }

        public void DecreaseSpeed(float deltaTime)
        {
            _elapsedTime += deltaTime;
            
            float k = Mathf.Log(_decayConstant) / _timeToDecrease;
            _currentVelocity = TargetVelocity * Mathf.Exp(-k * _elapsedTime);
            
            _movement.Move(_currentVelocity,deltaTime);
            
            if (_elapsedTime >= _timeToDecrease)
            {
                _currentVelocity = 0;
            }
        }
        
        public void RestartValues()
        {
            _elapsedTime = 0;
        }
    }
}