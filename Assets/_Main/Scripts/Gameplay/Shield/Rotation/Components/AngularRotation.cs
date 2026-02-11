using System;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.Components
{
    public class AngularRotation : IAngularRotation, AngularRotation.IRotation
    {
        private interface IRotation
        {
            public void DisableRotation();
            public void EnableRotation();
            public void Accelerate(float deltaTime);
            public bool GetHasReachedMaxSpeed();
            public bool GetHasReachedTargetAngle();
            public void SnapToTargetAngle();
            public bool GetHasTargetAngle();
            public void StopRotation();
            public void TriggerStartMoving();
            public void TriggerStopMoving();
        }
        
        private enum States
        {
            Disabled,
            Idle,
            Moving,
            Snapping,
            Stop
        }

        private class RotationController
        {
            private readonly IRotation _rotation;
            private States _currentState;

            public RotationController(IRotation rotation)
            {
                _rotation = rotation;
            }

            public void Execute(float deltaTime)
            {
                switch (_currentState)
                {
                    case States.Idle:
                        
                        if (_rotation.GetHasTargetAngle())
                        {
                            ChangeState(States.Moving);
                        }

                        break;
                    
                    case States.Moving:
                        
                        if(_rotation.GetHasReachedMaxSpeed() == false)
                            _rotation.Accelerate(deltaTime);

                        if (_rotation.GetHasReachedTargetAngle())
                        {
                            ChangeState(States.Snapping);
                        }

                        break;
                    
                    case States.Snapping:
                        
                        ChangeState(States.Idle);
                        
                        break;
                }
            }
            
            public void ChangeState(States newState)
            {
                if(_currentState == newState) return;
                
                // Sleep
                SleepState(_currentState);
                
                _currentState = newState;

                // Awake
                AwakeState(_currentState);
                
                Debug.Log("Current State: " + _currentState);
            }

            private void SleepState(States state)
            {
                switch (state)
                {
                    case States.Moving:
                        
                        _rotation.TriggerStopMoving();
                        
                        break;
                    
                    case States.Disabled:
                        
                        _rotation.EnableRotation();
                        
                        break;
                }
            }

            private void AwakeState(States state)
            {
                switch (state)
                {
                    case States.Disabled:
                        _rotation.DisableRotation();
                        break;
                    
                    case States.Moving:
                        
                        _rotation.TriggerStartMoving();
                        
                        break;
                    
                    case States.Snapping:

                        _rotation.StopRotation();
                        _rotation.SnapToTargetAngle();
                        
                        break;
                    
                    case  States.Stop:

                        _rotation.StopRotation();
                        
                        break;
                }
            }
        }

        private readonly Transform _objectToRotate;
        private readonly IAngularRotationData _data;
        private readonly RotationController _controller;
        private float _inputAngle;
        private float _inputMagnitude;
        private float _targetAngle;
        private float _currentAcc;
        private bool _hasTargetAngle;
        private bool _isInputEnable;
        private int _targetSlot;
        
        public event Action OnStopped;
        public event Action OnMoved;

        public AngularRotation(Transform objectToRotate, IAngularRotationData data)
        {
            _objectToRotate = objectToRotate;
            _data = data;
            _controller = new RotationController(this);
            _controller.ChangeState(States.Disabled);
        }

        #region Private API
        
        private float GetMaxSpeed() => _data.MaxSpeed * GetSpeedMagnitudeCurve();
        private float GetSpeedMagnitudeCurve() => _data.MagnitudeCurve.Evaluate(_inputMagnitude);
        private void ClampAngularVelocity() => _currentAcc = Mathf.Clamp(_currentAcc, 0, GetMaxSpeed());
        
        private void RotateObject(float deltaTime)
        {
            var targetAngle = Quaternion.Euler(0, 0, _targetAngle);
            var currentAngle = _objectToRotate.rotation;
            var finalSpeed = _currentAcc * deltaTime;
            var finalRotation = Quaternion.RotateTowards(currentAngle ,targetAngle, finalSpeed);
            _objectToRotate.rotation = finalRotation;
        }

        #endregion

        #region Public Shared

        public void StopRotation()
        {
            _currentAcc = 0;
            _hasTargetAngle = false;
            Debug.Log("Rotation Stopped");
        }

        #endregion
        
        #region IAngularRotation

        public void Execute(float deltaTime)
        {
            _controller.Execute(deltaTime);
            ClampAngularVelocity();
            RotateObject(deltaTime);
        }
        
        public void SetInputAngle(float inputAngle)
        {
            if(_isInputEnable == false) return;
            
            var temp = Mathf.Abs(inputAngle - _inputAngle);
            
            if (temp < _data.MinInputAngle)
            {
                return;
            }

            _hasTargetAngle = true;
            _inputAngle = inputAngle;
            _targetAngle = inputAngle;
        }
        
        public void SetInputMagnitude(float inputMagnitude)
        {
            if(_isInputEnable == false) return;
            
            if (Mathf.Approximately(_inputMagnitude, inputMagnitude))
            {
                return;
            }
            
            _inputMagnitude = inputMagnitude;
        }

        public void SetEnable(bool isEnabled)
        {
            _controller.ChangeState(isEnabled ? States.Idle : States.Disabled);
        }

        public void RestartPosition()
        {
            _objectToRotate.rotation = Quaternion.Euler(0, 0, 0);
        }

        public void SetActiveInput(bool isActive)
        {
            _isInputEnable = isActive;
        }

        #endregion

        #region IRotation

        public bool GetHasTargetAngle() => _hasTargetAngle;
        public bool GetHasReachedMaxSpeed() => GetSpeedRatio() >= 1;
        public float GetSpeedRatio() => Mathf.Clamp01(_currentAcc / GetMaxSpeed());
        public float GetCurrentAngle() => _objectToRotate.rotation.eulerAngles.z;
        
        public bool GetHasReachedTargetAngle()
        {
            const float angleThreshold = 0.5f;
            return Mathf.Abs(GetCurrentAngle() - _targetAngle) <= angleThreshold;
        }

        public void DisableRotation()
        {
            StopRotation();
            SetActiveInput(false);
        }

        public void EnableRotation()
        {
            SetActiveInput(true);
            _inputAngle = float.MaxValue;
        }

        public void Accelerate(float deltaTime)
        {
            var finalAcceleration = _data.Acceleration * GetSpeedMagnitudeCurve();
            _currentAcc += finalAcceleration * deltaTime;
        }
        
        public void SnapToTargetAngle() => _objectToRotate.rotation = Quaternion.Euler(0, 0, _targetAngle);

        public void TriggerStartMoving() => OnMoved?.Invoke();
        public void TriggerStopMoving() => OnStopped?.Invoke();

        #endregion
    }
}