using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield._Experiment
{
    public class ShieldDegreeMovement
    {
        private readonly Transform _shieldTransform;
        private readonly ShieldDegreeMovementDataSo _data;
        
        private int _currentSlot = 0;
        private float _targetAngle = 0;
        private float _currentAngle = 0;
        
        private float _leftHoldTimer = 0f;
        private float _rightHoldTimer = 0f;
        private float _leftDelay;
        private float _rightDelay;


        public ShieldDegreeMovement(Transform shieldTransform, ShieldDegreeMovementDataSo data)
        {
            _shieldTransform = shieldTransform;
            _data = data;
            Initialize();
        }

        private void Initialize()
        {
            UpdateTargetAngle();
            _currentAngle = _targetAngle;
            _shieldTransform.localRotation = Quaternion.Euler(0, 0, _currentAngle);
            _leftDelay = _data.InitialDelay;
            _rightDelay = _data.InitialDelay;
        }

        public void Update(float deltaTime)
        {
            _currentAngle = Mathf.LerpAngle(_currentAngle, _targetAngle, _data.RotationSpeed * deltaTime);
            _shieldTransform.localRotation = Quaternion.Euler(0, 0, _currentAngle);
        }
        
        private void UpdateTargetAngle()
        {
            float anglePerSlot = 360f / _data.Slots;
            _targetAngle = _currentSlot * anglePerSlot;
        }
        
        private void HandleInput(ref float timer, ref float delay, float deltaTime, System.Action action = null)
        {
            timer += deltaTime;
            if (timer >= delay)
            {
                action?.Invoke();
                timer = 0f;
                delay = Mathf.Max(_data.MinDelay, delay - _data.AccelerationRate); 
            }
        }

        private void ClearData(ref float timer, ref float delay)
        {
            timer = 0f;
            delay = _data.InitialDelay;
        }

        private void MoveRight()
        {
            _currentSlot = (_currentSlot + 1) % _data.Slots;
        }

        private void MoveLeft()
        {
            _currentSlot = (_currentSlot - 1 + _data.Slots) % _data.Slots;
        }

        public void HandleMove(int direction, float deltaTime)
        {
            if (direction > 0)
            {
                HandleInput(ref _rightHoldTimer, ref _rightDelay, deltaTime, MoveRight);
                ClearData(ref _leftHoldTimer, ref _leftDelay);
            }
            else if (direction < 0)
            {
                HandleInput(ref _leftHoldTimer, ref _leftDelay, deltaTime, MoveLeft);
                ClearData(ref _rightHoldTimer, ref _rightDelay);
            }
            else
            {
                ClearData(ref _leftHoldTimer, ref _leftDelay);
                ClearData(ref _rightHoldTimer, ref _rightDelay);
            }
            
            UpdateTargetAngle();
        }
    }
}
