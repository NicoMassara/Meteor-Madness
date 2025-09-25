using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield._Experiment
{
    public class ShieldDegreeMovement
    {
        private Transform _shieldTransform;
        private readonly int _slots = 16;
        private readonly float moveSpeed = 20f;
        private readonly float repeatDelay = 0.05f; 

        private int _currentSlot = 0;
        private float _targetAngle = 0;
        private float _currentAngle = 0;
        private float _leftHoldTimer = 0f;
        private float _rightHoldTimer = 0f;
        
        private bool _isTryingToMoveRight = false;
        private bool _isTryingToMoveLeft = false;


        public ShieldDegreeMovement(Transform shieldTransform, float moveSpeed, float repeatDelay, int slots)
        {
            _shieldTransform = shieldTransform;
            this.moveSpeed = moveSpeed;
            this.repeatDelay = repeatDelay;
            this._slots = slots * 4;
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
            _currentAngle = Mathf.LerpAngle(_currentAngle, _targetAngle, moveSpeed * deltaTime);
            _shieldTransform.localRotation = Quaternion.Euler(0, 0, _currentAngle);
        }

        private void UpdateTargetAngle()
        {
            float anglePerSlot = 360f / _slots;
            _targetAngle = _currentSlot * anglePerSlot;
        }

        public void Move(int direction, float deltaTime)
        {
            if (direction > 0)
            {
                MoveRight(deltaTime);
                _leftHoldTimer = repeatDelay;
            }
            else if (direction < 0)
            {
                MoveLeft(deltaTime);
                _rightHoldTimer = repeatDelay;
            }
            else
            {
                _leftHoldTimer = repeatDelay;
                _rightHoldTimer = repeatDelay;
            }

            UpdateTargetAngle();
        }

        private void MoveRight(float deltaTime)
        {
            _rightHoldTimer += deltaTime;
            
            if (_rightHoldTimer >= repeatDelay)
            {
                _currentSlot = (_currentSlot + 1 + _slots) % _slots;
                _rightHoldTimer = 0f;
            }
        }

        private void MoveLeft(float deltaTime)
        {
            _leftHoldTimer += deltaTime;
            
            if (_leftHoldTimer >= repeatDelay)
            {
                _currentSlot = (_currentSlot - 1 + _slots) % _slots;
                _leftHoldTimer = 0f;
            }

        }
    }
}