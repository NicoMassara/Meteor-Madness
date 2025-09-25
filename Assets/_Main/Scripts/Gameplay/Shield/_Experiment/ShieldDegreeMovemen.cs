using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield._Experiment
{
    public class ShieldDegreeMovement
    {
        private Transform _shieldTransform;
        private const int Slots = 16;
        private const float MoveSpeed = 10f;
        private const float RepeatDelay = 0.1f; 

        private int _currentSlot = 0;
        private float _targetAngle = 0;
        private float _currentAngle = 0;
        private float _leftHoldTimer = 0f;
        private float _rightHoldTimer = 0f;
        
        private bool _isTryingToMoveRight = false;
        private bool _isTryingToMoveLeft = false;


        public ShieldDegreeMovement(Transform shieldTransform)
        {
            _shieldTransform = shieldTransform;
            Initialize();
        }

        private void Initialize()
        {
            UpdateTargetAngle();
            _currentAngle = _targetAngle;
            _shieldTransform.localRotation = Quaternion.Euler(0, 0, _currentAngle);
        }

        public void Update(float deltaTime)
        {
            _currentAngle = Mathf.LerpAngle(_currentAngle, _targetAngle, MoveSpeed * deltaTime);
            _shieldTransform.localRotation = Quaternion.Euler(0, 0, _currentAngle);
        }

        private void UpdateTargetAngle()
        {
            float anglePerSlot = 360f / Slots;
            _targetAngle = _currentSlot * anglePerSlot;
        }

        public void Move(int direction, float deltaTime)
        {
            if(direction > 0) MoveRight(deltaTime);
            else if(direction < 0) MoveLeft(deltaTime);
            
            UpdateTargetAngle();
        }
        
        private void MoveRight(float deltaTime)
        {
            _rightHoldTimer += deltaTime;
            
            if (_rightHoldTimer >= RepeatDelay)
            {
                _currentSlot = (_currentSlot + 1 + Slots) % Slots;
                _rightHoldTimer = 0f;
            }
        }

        private void MoveLeft(float deltaTime)
        {
            _leftHoldTimer += deltaTime;
            
            if (_leftHoldTimer >= RepeatDelay)
            {
                _currentSlot = (_currentSlot - 1 + Slots) % Slots;
                _leftHoldTimer = 0f;
            }

        }
    }
}