using System;
using System.Collections.Generic;
using _Main.Scripts.GlobalValues.Tools;
using UnityEngine;

namespace _Main.Scripts.Movement
{
    public class MovementComponent :
        IMovement,
        MovementComponent.IFsmMovement,
        IDebugMovementComponent
    {
        #region FSM

        #region Enum

        private enum States
        {
            None,
            Stationary,
            Accelerate,
            Snap,
            DeAccelerate
        }

        #endregion

        #region Interfaces

        private interface IFsmMovement
        {
            // === Data === //
            public bool GetHasReachedMaxSpeed();
            public float GetTravelDistanceRatio();
            public float GetInputAngle();
            public float GetCurrentAngle();
            public float GetTargetAngle();
            public float GetMaxTravelDistanceRatioToDeAccelerate();
            public float GetMinDistanceRatioToSnap();
            public float GetSpeedRatio();
            public bool GetHasInput();
            public bool GetHasReachedTargetAngle();
            
            // === Actions === //
            public void Accelerate(float deltaTime);
            public void DeAccelerate(float deltaTime, float distanceRatio);
            public void ForceSnapToTargetAngle();
            public void ForceStopMovement();
            public void SetTargetAngle(float newAngle);
            public void UpdateTargetAngle(float deltaTime, float inputAngle);
            public void SetHasTargetAngle(bool hasTargetAngle);
        }

        private interface IFsmTransitions : IFsmControllerTransitions
        {
            public void TransitionToStationary();
            public void TransitionToAccelerate();
            public void TransitionToSnap();
            public void TransitionToDeAccelerate();
        }

        #endregion

        #region States

        private class BaseState : FsmState<IFsmMovement, IFsmTransitions> { }
        private class StationaryState : BaseState
        {
            public override void Execute(float deltaTime)
            {
                if (Controller.GetHasInput())
                {
                    Controller.SetTargetAngle(Controller.GetInputAngle());
                    Transitions.TransitionToAccelerate();
                    return;
                }
            }
        }
        private class AccelerateState : BaseState
        {
            public override void Execute(float deltaTime)
            {
                Controller.UpdateTargetAngle(deltaTime, Controller.GetInputAngle());
                
                if (Controller.GetHasReachedMaxSpeed() == false)
                {
                    Controller.Accelerate(deltaTime);
                }
                
                if (Controller.GetHasReachedTargetAngle())
                {
                    Transitions.TransitionToSnap();
                    return;
                }
                
                if (Controller.GetTravelDistanceRatio() >= Controller.GetMaxTravelDistanceRatioToDeAccelerate())
                {
                    Transitions.TransitionToDeAccelerate();
                    return;
                }
            }
        }
        private class SnapState : BaseState
        {
            public override void Awake()
            {
                Controller.ForceStopMovement();
                Controller.SetHasTargetAngle(false);
                Controller.ForceSnapToTargetAngle();
                
                Transitions.TransitionToStationary();
            }
        }
        private class DeAccelerateState : BaseState
        {
            private float _startTarget;
            private float _startAngle;
            private float _startDistance;
            
            public override void Awake()
            {
                _startTarget = Controller.GetTargetAngle();
                _startAngle = Controller.GetCurrentAngle();
                _startDistance = GetDistanceToTarget(_startAngle);
                
                if (_startDistance <= 0)
                {
                    Transitions.TransitionToSnap();
                }
            }

            public override void Execute(float deltaTime)
            {
                Controller.UpdateTargetAngle(deltaTime, Controller.GetInputAngle());

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_startTarget != Controller.GetTargetAngle())
                {
                    Transitions.TransitionToAccelerate();
                    return;
                }
                
                var currDist = GetDistanceToTarget(Controller.GetCurrentAngle());
                float distanceRatio = Mathf.Clamp01(currDist / _startDistance);
                
                Controller.DeAccelerate(deltaTime, distanceRatio);

                if ((1f - distanceRatio) >= Controller.GetMinDistanceRatioToSnap() ||
                    Controller.GetSpeedRatio() <= 0.001f)
                {
                    Transitions.TransitionToSnap();
                    return;
                }
            }

            private float GetDistanceToTarget(float current) 
                => Mathf.Abs(Mathf.DeltaAngle(current, Controller.GetTargetAngle()));
        }

        #endregion

        #region Controller

        private class FsmController : GenericSimpleFsm<States>, IFsmTransitions
        {
            public FsmController()
            {
            }
            
            public void Initialize(IFsmMovement movementComponent)
            {
                var temp = new List<StateData<BaseState>>
                {
                    new (States.Stationary, new StationaryState()),
                    new (States.Accelerate, new AccelerateState()),
                    new (States.Snap, new SnapState()),
                    new (States.DeAccelerate, new DeAccelerateState()),
                };

                foreach (var state in temp)
                {
                    state.State.InitializeState(movementComponent,this);
                }

                InitializeStates(temp);
                
                TransitionToStationary();
            }

            public void TransitionToStationary() => Transition(States.Stationary);
            public void TransitionToAccelerate() => Transition(States.Accelerate);
            public void TransitionToSnap() => Transition(States.Snap);
            public void TransitionToDeAccelerate() => Transition(States.DeAccelerate);
        }

        #endregion

        #endregion
        
        private readonly Transform _transform;
        private readonly IMovementData _movementData;
        private readonly FsmController _controller;
        private float _inputAngle;
        private float _currentDirection;
        private float _targetAngle;
        private float _currentAcceleration;
        private float _distanceToTargetAngle;
        private float _inputMagnitude;
        private float _lastSpeedRatio;
        private bool _hasInput;
        private bool _hasTargetAngle;
        private bool _hasToChangeDirection;

        #region IMovement
        public event Action OnStopped;
        public event Action OnMoved;
        public event Action OnDirectionChanged;

        #endregion
            
        #region IDebugMovementComponent

        public bool HasTargetAngle => _hasTargetAngle;
        public float CurrentSpeed => _currentAcceleration;
        public float SpeedRatio => GetSpeedRatio();
        public float CurrentAngle => _transform.eulerAngles.z;
        public float CurrentDirection => _currentDirection;
        public float TargetAngle => _targetAngle;
        public bool HasToChangeDirection => _hasToChangeDirection;
        public string CurrentState { get; private set; }
        public string LastState { get; private set; }
        public float DistanceToTarget => DistanceToTargetAngle();
        public float DistanceToTargetRatio => GetTravelDistanceRatio();
        public bool HasInput => _hasInput;
        public float CurveMagnitude => GetSpeedMagnitudeCurve();

        #endregion
        
        public MovementComponent(Transform transform, IMovementData movementData)
        {
            _controller = new FsmController();
            _transform = transform;
            _movementData = movementData;

            CurrentState = "None";
            LastState = "None";
        }

        #region Private API

        private float GetMaxSpeed() => _movementData.MaxSpeed * GetSpeedMagnitudeCurve();
        private float GetSpeedMagnitudeCurve() => _movementData.MagnitudeCurve.Evaluate(_inputMagnitude);

        private void ClampAngularVelocity()
        {
            _currentAcceleration = Mathf.Clamp(_currentAcceleration, 0, GetMaxSpeed());
        }

        private void RotateObject(float deltaTime)
        {
            var targetAngle = Quaternion.Euler(0, 0, _targetAngle);
            var currentAngle = _transform.rotation;
            var finalSpeed = _currentAcceleration * deltaTime;
            var finalRotation = Quaternion.RotateTowards(currentAngle ,targetAngle, finalSpeed);
            _transform.rotation = finalRotation;
        }
        
        private void SetDistanceToTargetAngle()
        {
            _distanceToTargetAngle = DistanceToTargetAngle();
        }
        
        private float DistanceToTargetAngle()
            => Mathf.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _targetAngle));

        private void DisableInput()
        {
            _hasInput = false;
            _inputMagnitude = 1f;
        }

        #endregion

        #region IMovement
        
        public void Initialize()
        {
            _controller.OnStateChanged += OnStateChangedHandler;
            _controller.Initialize(this);
        }

        public void SetInputAngle(float inputAngle)
        {
            var temp = Mathf.Abs(inputAngle - _inputAngle);
            
            if (temp < _movementData.MinInputAngle)
            {
                return;
            }
            
            _inputAngle = inputAngle;
            _hasInput = true;
        }
        
        public void SetInputMagnitude(float inputMagnitude)
        {
            if (Mathf.Approximately(_inputMagnitude, inputMagnitude))
            {
                return;
            }
            
            _inputMagnitude = inputMagnitude;
        }

        public void Update(float deltaTime)
        {
            _controller?.Execute(deltaTime);
            ClampAngularVelocity();
            RotateObject(deltaTime);
            
            if (_lastSpeedRatio < 0.1f && GetSpeedRatio() >= 0.1f)
            {
                OnMoved?.Invoke();
            }
            
            if (_lastSpeedRatio > 0.1f && GetSpeedRatio() <= 0.1f)
            {
                OnStopped?.Invoke();
            }

            _lastSpeedRatio = GetSpeedRatio();
        }

        public void RemoveInput()
        {
            DisableInput();
        }

        public void ForceStop()
        {
            RemoveInput();
            ForceStopMovement();
            _controller.TransitionToStationary();
        }

        #endregion

        #region IFsmMovement

        #region Data
        public bool GetHasReachedTargetAngle()
        {
            const float angleThreshold = 0.1f;
            return Mathf.Abs(GetCurrentAngle() - _targetAngle) <= angleThreshold;
        }

        public bool GetHasTargetAngle() => _hasTargetAngle;
        public bool GetHasToChangeDirection() => _hasToChangeDirection;
        public bool GetHasReachedMaxSpeed() => GetSpeedRatio() >= 1;
        public float GetSpeedRatio() => Mathf.Clamp01(_currentAcceleration / GetMaxSpeed());
        public float GetTravelDistanceRatio() => Mathf.Clamp01(1f - (DistanceToTargetAngle() / _distanceToTargetAngle));
        public float GetInputAngle() => _inputAngle;
        public bool GetHasInput() => _hasInput;
        public float GetTargetAngle() => _targetAngle;
        public float GetMaxTravelDistanceRatioToDeAccelerate() => _movementData.MaxTravelDistanceRatioToDeAccelerate;
        public float GetMinDistanceRatioToSnap() => _movementData.MinDistanceRatioToSnap;
        public float GetCurrentAngle() => _transform.rotation.eulerAngles.z;

        #endregion

        #region Actions
        
        public void UpdateTargetAngle(float deltaTime, float inputAngle)
        {
            if(_hasTargetAngle)
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_targetAngle == inputAngle)
                {
                    return;
                }

            _targetAngle = inputAngle;
            SetDistanceToTargetAngle();
            
            float directionDelta = Mathf.DeltaAngle(GetCurrentAngle(), _targetAngle);

            if (_currentDirection != 0)
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_currentDirection != directionDelta)
                {
                    OnDirectionChanged?.Invoke();
                }

            _currentDirection = Mathf.Sign(directionDelta);
            _hasTargetAngle = true;
        }

        public void Accelerate(float deltaTime)
        {
            var finalAcceleration = _movementData.Acceleration * GetSpeedMagnitudeCurve();
            _currentAcceleration += finalAcceleration * deltaTime;
        }

        public void DeAccelerate(float deltaTime, float distanceRatio)
        {
            var finalDeAcceleration = _movementData.DeAcceleration * distanceRatio;
            _currentAcceleration -= finalDeAcceleration * deltaTime;
        }

        public void ForceSnapToTargetAngle()
        {
            _transform.rotation = Quaternion.Euler(0, 0, _targetAngle);
        }
        public void SetHasTargetAngle(bool hasTargetAngle)
        {
            _hasTargetAngle = hasTargetAngle;
            _hasInput = hasTargetAngle;
        }

        public void ForceStopMovement()
        {
            _currentAcceleration = 0;
            _currentDirection = 0;
        }

        public void SetTargetAngle(float newAngle)
        {
            _targetAngle = newAngle;
            SetDistanceToTargetAngle();
        }

        #endregion

        #endregion

        #region Handlers

        private void OnStateChangedHandler(States state)
        {
            LastState = CurrentState;
            CurrentState = state.ToString();
        }

        #endregion
    }
}