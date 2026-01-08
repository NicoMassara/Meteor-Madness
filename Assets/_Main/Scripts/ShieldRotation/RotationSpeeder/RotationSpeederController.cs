using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.ShieldRotation.Contracts;
using _Main.Scripts.ShieldRotation.Tools;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.RotationSpeeder
{
    public class RotationSpeederController : 
        IRotationSpeeder,
        RotationSpeederController.IFsmSpeeder
    {
        #region FSM
        private enum States
        {
            Idle,
            SpeedUp,
            Rotating,
            SpeedDown
        }

        private interface IFsmSpeeder
        {
            // === Data Getters === //

            public bool GetHasReachedMaxSpeed();
            public bool GetHasReachedMinSpeed();
            
            // === Actions === // 
            
            public void InitializeDecreaseSpeed();
            public void InitializeIncreaseSpeed();
            public void IncreaseSpeed(float deltaTime);
            public void DecreaseSpeed(float deltaTime);
            public void ClearSpeed();
            
            // === Triggers === //
            
            public void TriggerOnReachedMaxSpeed();
            public void TriggerOnReachedMinSpeed();
            
            // === Transitions === // 
            public void TransitionToIdle();
            public void TransitionToRotating();
        }
        
        private abstract class SpeederStateBase : StateBase<IFsmSpeeder> { }

        #region States

        private class IdleState : SpeederStateBase { }
        
        private class SpeedUpState : SpeederStateBase
        {
            public override void Awake()
            {
                Controller.InitializeIncreaseSpeed();
            }

            public override void Execute(float deltaTime)
            {
                Controller.IncreaseSpeed(deltaTime);
                
                if (Controller.GetHasReachedMaxSpeed())
                {
                    Controller.TriggerOnReachedMaxSpeed();
                    Controller.TransitionToRotating();
                    return;
                }
            }
        }

        private class RotatingState : SpeederStateBase
        {
            public override void Awake()
            {
                Controller.ClearSpeed();
            }
        }
        private class SpeedDownState : SpeederStateBase
        {
            public override void Awake()
            {
                Controller.InitializeDecreaseSpeed();
            }
            
            public override void Execute(float deltaTime)
            {
                Controller.DecreaseSpeed(deltaTime);
                
                if (Controller.GetHasReachedMinSpeed())
                {
                    Controller.TriggerOnReachedMinSpeed();
                    Controller.TransitionToIdle();
                    return;
                }
            }
        }

        #endregion
        
        #region Controller

        private class SpeederFsm : FsmController<States>
        {
            public SpeederFsm(IFsmSpeeder speeder)
            {
                Initialize(speeder);
            }

            private void Initialize(IFsmSpeeder speeder)
            {
                var tempList = new List<StateData>
                {
                    new (States.Idle, new IdleState()),
                    new (States.SpeedUp, new SpeedUpState()),
                    new (States.Rotating, new RotatingState()),
                    new (States.SpeedDown, new SpeedDownState())
                };

                foreach (var state in tempList.Select(item => (SpeederStateBase)item.State))
                {
                    state.InitializeState(speeder);
                }
                
                InitializeStates(tempList);
            }
        }

        #endregion
        
        #endregion
        
        private readonly SpeederFsm _fsmController;
        private readonly IRotationSpeederData _data;
        private readonly Transform _objectToRotate;
        private float _angularSpeed;
        private float _targetMinSpeed;
        private float _totalDegrees;
        private float _targetDegreesStep;
        private float _lastAngle;
        
        private float SpeedRatio => Mathf.Abs(_angularSpeed) / _data.MaxSpeed;
        
        public float AngularSpeed => _angularSpeed;
        public event Action OnReachedMaxSpeed;
        public event Action OnReachedMinSpeed;
        public event Action OnSpeedIncreased;
        public event Action OnSpeedDecreased;

        public RotationSpeederController(IRotationSpeederData data, Transform objectToRotate)
        {
            _data = data;
            _fsmController = new SpeederFsm(this);
            _objectToRotate = objectToRotate;
        }
        
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
        
        private void RotateObject(float deltaTime) => _objectToRotate.Rotate(0f, 0f, _angularSpeed * deltaTime);

        private float GetAccelerateTotalDegrees() => _data.AccelerateTurnsAmount * 360f;
        private float GetAngularAcceleration() => (_data.MaxSpeed * _data.MaxSpeed) / (2f * GetAccelerateTotalDegrees());
        
        private float GetDeAccelerateTotalDegrees() => _data.DeAccelerateTurnsAmount * 360f;
        private float GetAngularDeAcceleration() => (_targetMinSpeed * _targetMinSpeed - _data.MaxSpeed * _data.MaxSpeed) / 
                                                    (2f * GetDeAccelerateTotalDegrees());
        
        private float GetCurrentAngle() => _objectToRotate.localEulerAngles.z;

        #region IRotationSpeeder

        public void Update(float deltaTime)
        {
            _fsmController?.Execute(deltaTime);
            
            ClampAngularSpeed();
            RotateObject(deltaTime);
        }
        
        public void SpeedUp() => _fsmController.Transition(States.SpeedUp);
        public void SlowDown(float targetMinSpeed)
        {
            _targetMinSpeed = targetMinSpeed;
            _fsmController.Transition(States.SpeedDown);
        }

        #endregion

        #region IFsmSpeeder
        
        // === Data Getters === //
        
        #region Data Getters

        public bool GetHasReachedMaxSpeed() => SpeedRatio >= 1;
        public bool GetHasReachedMinSpeed() => GetMinSpeedRatio() <= 0;

        #endregion

        // === Actions === // 

        #region Actions

        public void ClearSpeed()
        {
            //_angularSpeed = 0;
        }

        public void InitializeIncreaseSpeed()
        {
            _totalDegrees = 0;
            _lastAngle = GetCurrentAngle();
            _targetDegreesStep = _data.DegreesStep;
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
                OnSpeedIncreased?.Invoke();
            }
        }
        
        public void InitializeDecreaseSpeed()
        {
            _angularSpeed = _data.MaxSpeed;
            _totalDegrees = 0;
            _targetDegreesStep = _data.DegreesStep;
            _lastAngle = GetCurrentAngle();
        }

        public void DecreaseSpeed(float deltaTime)
        {
            float currentAngle = GetCurrentAngle();
            float deltaAngle = Mathf.DeltaAngle(_lastAngle, GetCurrentAngle());
            
            _angularSpeed += GetAngularDeAcceleration() * deltaTime;
            
            _totalDegrees += Mathf.Abs(deltaAngle);
            _lastAngle = currentAngle;
            
            if (_totalDegrees >= _targetDegreesStep)
            {
                _targetDegreesStep += _data.DegreesStep;
                OnSpeedDecreased?.Invoke();
            }
        }

        #endregion
        
        // === Triggers === // 

        #region Triggers

        public void TriggerOnReachedMaxSpeed() => OnReachedMaxSpeed?.Invoke();

        public void TriggerOnReachedMinSpeed() => OnReachedMinSpeed?.Invoke();

        #endregion
        
        // === Transitions === //
        
        #region Transitions

        public void TransitionToIdle() => _fsmController.Transition(States.Idle);
        public void TransitionToRotating() => _fsmController.Transition(States.Rotating);

        #endregion

        #endregion
    }
}