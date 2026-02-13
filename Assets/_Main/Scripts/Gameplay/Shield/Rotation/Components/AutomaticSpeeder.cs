using System;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.Components
{
    public class AutomaticSpeeder : IAutomaticSpeeder, AutomaticSpeeder.ISpeeder
    {
        private interface ISpeeder
        {
            // Initializers
            public void InitializeSpeedUpValues();
            public void InitializeSpeedDownValues();
            
            // Actions
            public void RotateObject(float deltaTime);
            public void IncreaseSpeed(float deltaTime);
            public void DecreaseSpeed(float deltaTime);
            
            // Data Getters
            public bool GetHasReachedMaxSpeed();
            public bool GetHasReachedMinSpeed();
            
            // Triggers
            public void TriggerOnReachedMaxSpeed();

            public void TriggerOnReachedMinSpeed();
        }
        
        private enum States
        {
            Idle,
            SpeedUp,
            Rotating,
            SpeedDown
        }

        private class SpeederController
        {
            private readonly ISpeeder _speeder;
            private States _currentState;

            public SpeederController(ISpeeder speeder)
            {
                _speeder = speeder;
            }


            public void ChangeState(States newState)
            {
                if(_currentState == newState) return;
                
                // Sleep
                Sleep(_currentState);
                
                _currentState = newState;

                // Awake
                Awake(_currentState);
            }
            
            public void Execute(float deltaTime)
            {
                switch (_currentState)
                {
                    case States.SpeedUp:
                        
                        _speeder.IncreaseSpeed(deltaTime);
                        _speeder.RotateObject(deltaTime);

                        if (_speeder.GetHasReachedMaxSpeed())
                        {
                            ChangeState(States.Rotating);
                        }

                        break;
                    
                    case States.Rotating:
                        
                        //_speeder.RotateObject(deltaTime);
                        
                        break;

                    case States.SpeedDown:
                        
                        _speeder.DecreaseSpeed(deltaTime);
                        _speeder.RotateObject(deltaTime);

                        if (_speeder.GetHasReachedMinSpeed())
                        {
                            ChangeState(States.Idle);
                        }
                        
                        break;
                }
            }
            
            private void Awake(States state)
            {
                switch (state)
                {
                    case States.SpeedUp:
                        
                        _speeder.InitializeSpeedUpValues();
                        
                        break;
                    case States.SpeedDown:
                        
                        _speeder.InitializeSpeedDownValues();
                        
                        break;
                }
            }

            private void Sleep(States state)
            {
                switch (state)
                {
                    case States.SpeedUp:
                        
                        _speeder.TriggerOnReachedMaxSpeed();
                        
                        break;
                    case States.SpeedDown:
                        
                        _speeder.TriggerOnReachedMinSpeed();
                        
                        break;
                }
            }

        }

        private readonly IAutomaticSpeederData _data;
        private readonly Transform _objectToRotate;
        private readonly SpeederController _controller;
        private float _angularSpeed;
        private float _targetMinSpeed;
        private float _totalDegrees;
        private float _targetDegreesStep;
        private float _lastAngle;
        
        public event Action OnReachedMaxSpeed;
        public event Action OnReachedMinSpeed;
        public event Action OnSpeedIncreased;
        public event Action OnSpeedDecreased;

        public AutomaticSpeeder(Transform objectToRotate, IAutomaticSpeederData data)
        {
            _data = data;
            _objectToRotate = objectToRotate;
            _controller = new  SpeederController(this);
            _controller.ChangeState(States.Idle);
        }

        #region Private API
        private void ClampAngularSpeed()
        {
            _angularSpeed = Mathf.Clamp(_angularSpeed, -_data.MaxSpeed, _data.MaxSpeed);
        }
        
        private float Inverse01(float min, float max, float value)
        {
            if (min == max) return 0f;
            float t = (value - min) / (max - min);
            return Mathf.Clamp01(t);
        }
        
        private float GetMinSpeedRatio() => Inverse01(_targetMinSpeed, _data.MaxSpeed, Mathf.Abs(_angularSpeed));
        private float GetMaxSpeedRatio() => Mathf.Abs(_angularSpeed) / _data.MaxSpeed;

        private float GetAccelerateTotalDegrees() => _data.AccelerateTurnsAmount * 360f;
        private float GetAngularAcceleration() => (_data.MaxSpeed * _data.MaxSpeed) / (2f * GetAccelerateTotalDegrees());
        
        private float GetDeAccelerateTotalDegrees() => _data.DeAccelerateTurnsAmount * 360f;
        private float GetAngularDeAcceleration() => (_targetMinSpeed * _targetMinSpeed - _data.MaxSpeed * _data.MaxSpeed) / 
                                                    (2f * GetDeAccelerateTotalDegrees());
        
        private float GetCurrentAngle() => _objectToRotate.localEulerAngles.z;

        #endregion
        
        #region IAutomaticSpeeder

        public void Execute(float deltaTime)
        {
            _controller.Execute(deltaTime);
        }

        public void SpeedUp()
        {
            _controller.ChangeState(States.SpeedUp);
        }

        public void SpeedDown(float targetSpeed)
        {
            _targetMinSpeed = targetSpeed;
            _controller.ChangeState(States.SpeedDown);
        }

        #endregion

        #region IController

        // === Initializers === // 
        public void InitializeSpeedUpValues()
        {
            _totalDegrees = 0;
            _lastAngle = GetCurrentAngle();
            _targetDegreesStep = _data.DegreesStep;
        }

        public void InitializeSpeedDownValues()
        {
            _angularSpeed = _data.MaxSpeed;
            _totalDegrees = 0;
            _targetDegreesStep = _data.DegreesStep;
            _lastAngle = GetCurrentAngle();
        }
        
        public void IncreaseSpeed(float deltaTime)
        {
            float currentAngle = GetCurrentAngle();
            float deltaAngle = Mathf.DeltaAngle(_lastAngle, GetCurrentAngle());
            
            _angularSpeed += GetAngularAcceleration() * deltaTime;
            
            _totalDegrees += Mathf.Abs(deltaAngle);
            _lastAngle = currentAngle;
            
            if (_totalDegrees >= _targetDegreesStep)
            {
                _targetDegreesStep += _data.DegreesStep;
                OnSpeedDecreased?.Invoke();
            }
            
            ClampAngularSpeed();
        }
        
        // === Actions === // 
        public void DecreaseSpeed(float deltaTime)
        {
            float currentAngle = GetCurrentAngle();
            float deltaAngle = Mathf.DeltaAngle(_lastAngle, currentAngle);
            
            _angularSpeed += GetAngularDeAcceleration() * deltaTime;
            
            _totalDegrees += Mathf.Abs(deltaAngle);
            _lastAngle = currentAngle;
            
            if (_totalDegrees >= _targetDegreesStep)
            {
                _targetDegreesStep += _data.DegreesStep;
                OnSpeedIncreased?.Invoke();
            }
            
            ClampAngularSpeed();
        }
        
        public void RotateObject(float deltaTime) => _objectToRotate.Rotate(0f, 0f, _angularSpeed * deltaTime);
        
        // === Data Getters === // 
        public bool GetHasReachedMaxSpeed() => GetMaxSpeedRatio() >= 1;
        public bool GetHasReachedMinSpeed() => GetMinSpeedRatio() <= 0;
        
        // === Triggers === //
        
        public void TriggerOnReachedMaxSpeed() => OnReachedMaxSpeed?.Invoke();
        public void TriggerOnReachedMinSpeed() => OnReachedMinSpeed?.Invoke();
        
        #endregion
        
    }
}