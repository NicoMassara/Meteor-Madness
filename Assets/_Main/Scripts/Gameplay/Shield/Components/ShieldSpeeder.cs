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
        private bool _isSpeedingUp;

        private const float TargetVelocity = 150f;

        public ShieldSpeeder(ShieldMovement movement, float timeToIncrease, float timeToDecrease, float decayConstant)
        {
            _movement = movement;
            _timeToIncrease = timeToIncrease;
            _timeToDecrease = timeToDecrease;
            _decayConstant = decayConstant;
        }

        public bool GetIsSpeedingUp()
        {
            return _isSpeedingUp;
        }

        public void IncreaseSpeed(float deltaTime)
        {
            _isSpeedingUp = true;
            _elapsedTime += deltaTime;
            var timeRatio = _elapsedTime / _timeToIncrease;
            
            if (timeRatio >= 1)
            {
                _isSpeedingUp = false;
            }
            
            _movement.HandleMove(1, deltaTime);
        }

        public void DecreaseSpeed(float deltaTime)
        {
            _isSpeedingUp = true;
            _elapsedTime += deltaTime;
            
            float k = Mathf.Log(_decayConstant) / _timeToDecrease;
            
            if (_elapsedTime >= _timeToDecrease)
            {
                _isSpeedingUp = false;
            }
            
            _movement.HandleMove(1, deltaTime);
        }
        
        public void RestartValues()
        {
            _elapsedTime = 0;
        }
    }
}