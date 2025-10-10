using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldSpeeder
    {
        private readonly ShieldMovementComponent movementComponent;
        private readonly float _timeToIncrease;
        private readonly float _timeToDecrease;
        private readonly float _decayConstant;
        private float _elapsedTime = 0;
        private bool _isSpeedingUp;

        private const float TargetVelocity = 150f;

        public ShieldSpeeder(ShieldMovementComponent movementComponent, float timeToIncrease, float timeToDecrease, float decayConstant)
        {
            this.movementComponent = movementComponent;
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
            
            movementComponent.HandleMove(1, deltaTime);
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
            
            movementComponent.HandleMove(1, deltaTime);
        }
        
        public void RestartValues()
        {
            _elapsedTime = 0;
        }
    }
}