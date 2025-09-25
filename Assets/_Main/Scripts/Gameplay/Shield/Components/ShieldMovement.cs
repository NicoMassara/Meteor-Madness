using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldMovement
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
        
        private bool _isPressingRight = false;
        private bool _isPressingLeft = false;

        private float _speedMultiplier = 1;


        public ShieldMovement(Transform shieldTransform, ShieldDegreeMovementDataSo data)
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
            if (_isPressingRight == false)
            {
                HandleDelay(ref _rightDelay, deltaTime);
            }
            if (_isPressingLeft == false)
            {
                HandleDelay(ref _leftDelay, deltaTime);
            }

            _currentAngle = Mathf.LerpAngle(_currentAngle, _targetAngle, GetFinalSpeed() * deltaTime);
            _shieldTransform.localRotation = Quaternion.Euler(0, 0, _currentAngle);
        }
        
        private void UpdateTargetAngle()
        {
            float anglePerSlot = 360f / GameValues.AngleSlots;
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

        private void ClearData(ref float timer, ref bool isPressing)
        {
            timer = 0f;
            isPressing = false;
        }

        private void HandleDelay(ref float delay, float deltaTime)
        {
            delay = Mathf.Lerp(delay, _data.InitialDelay, deltaTime * _data.DecelerationRate);
        }

        private void MoveRight()
        {
            _currentSlot = (_currentSlot + 1) % GameValues.AngleSlots;
        }

        private void MoveLeft()
        {
            _currentSlot = (_currentSlot - 1 + GameValues.AngleSlots) % GameValues.AngleSlots;
        }

        public void HandleMove(float direction, float deltaTime)
        {
            if (direction > 0)
            {
                HandleInput(ref _rightHoldTimer, ref _rightDelay, deltaTime, MoveRight);
                ClearData(ref _leftHoldTimer, ref _isPressingLeft);
            }
            else if (direction < 0)
            {
                HandleInput(ref _leftHoldTimer, ref _leftDelay, deltaTime, MoveLeft);
                ClearData(ref _rightHoldTimer, ref _isPressingRight);
            }
            else
            {
                ClearData(ref _leftHoldTimer, ref _isPressingLeft);
                ClearData(ref _rightHoldTimer, ref _isPressingRight);
            }
            
            UpdateTargetAngle();
        }

        private float GetFinalSpeed()
        {
            return _data.RotationSpeed * _speedMultiplier;
        }

        public void SetSpeedMultiplier(float speedMultiplier)
        {
            _speedMultiplier = Mathf.Clamp01(speedMultiplier);
        }

        public float GetAngle()
        {
            return Mathf.Repeat(_shieldTransform.rotation.eulerAngles.z, 360f);
        }

        public Vector2 GetPosition()
        {
            return new Vector2(_shieldTransform.position.x, _shieldTransform.position.y);
        }

        public int GetCurrentSlot()
        {
            return _currentSlot;
        }
    }
}
