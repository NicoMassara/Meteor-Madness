using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Main.Scripts.GameplayComponents.Movement
{
    public class MovementComponent : 
        IMovement,
        MovementComponent.IFsmMovement,
        MovementComponent.IMovementDebug
    {
        public interface IMovementDebug
        {
            public float AngularSpeed { get;}
            public string CurrentState { get; }
            public bool HasToCorrect { get; }
            public void StopMovement();
        }
        
        #region FSM
        
        private interface IFsmMovement
        {
            // === Data === //
            public float GetSpeedRatio();
            public float GetDirection();
            public bool GetIsMovingAtMaxSpeed();
            public float GetLastMovementSpeedRatio();
            public bool GetIsSnapFinished();
            public bool GetHasToCorrectSnap();
            public bool GetIsCorrectionSnapFinished();
            
            // === Setters === //

            public void SetIsStopping(bool isStopping);
            public void SetIsSnapping(bool isSnapping);
            public void StopCorrectionSnapping();
            
            // === Actions === //
            public void Stop();
            public void IncreaseSpeed(float deltaTime, float direction);
            public void ChangeDirection(float deltaTime, float getDirection);
            public void DecreaseSpeed(float deltaTime);
            public void SnapToSlot();
            public void SmoothSnapToSlot(float deltaTime);
            public void SaveLastMovementSpeedRatio();
            public void CalculateSnapAngle(bool shouldExtraSnap);
            public void HandleSnapCorrection(float deltaTime);
            
            // === Event Triggers === //
            
            public void TriggerOnStartMoving();
            public void TriggerOnDirectionChanged();
            public void TriggerOnStartStop();
            public void TriggerOnStopped();
            public void TriggerOnSnapCorrected();
            
            // === Transitions === //
            public void TransitionToStationary();
            public void TransitionToRotating();
            public void TransitionToChangingDirection();
            public void TransitionToStopping();
            public void TransitionSnapping();
            public void TransitionCorrectionSnapping();
        }
        
        #region States

        private enum States
        {
            None,
            Stationary,
            Rotating,
            ChangingDirection,
            Stopping,
            Snapping,
            CorrectionSnapping
        }
        
        private interface IState
        {
            public void Awake();
            public void Execute(float deltaTime);
            public void Sleep();
            
        }
        
        private abstract class StateBase : IState
        {
            protected IFsmMovement Movement { get; private set; }
            protected IFsmMovementData Data { get; private set; }

            public void Initialize(IFsmMovement movement, IFsmMovementData movementData)
            {
                Movement = movement;
                Data = movementData;
            }

            public virtual void Awake() { }
            public virtual void Execute(float deltaTime) { }
            public virtual void Sleep() { }
            
        }
        
        private class StationaryState : StateBase
        {
            public override void Execute(float deltaTime)
            {
                var hasInput = Movement.GetDirection() != 0;

                if (hasInput)
                {
                    Movement.TransitionToRotating();
                }
            }

            public override void Sleep()
            {
                Movement.TriggerOnStartMoving();
            }
        }
        private class RotatingState : StateBase
        {
            private float _stopTimer;
            private float _lastDirection;

            public override void Awake()
            {
                ResetStopTimer();
                _lastDirection = Movement.GetDirection();
            }

            public override void Execute(float deltaTime)
            {
                var currentDirection = Movement.GetDirection();
                var hasInput = currentDirection != 0;
                var direction = hasInput ? currentDirection : _lastDirection;
                
                if(currentDirection != 0  && _lastDirection != direction)
                {
                    Movement.TransitionToChangingDirection();
                    return;
                }
                
                _lastDirection = direction;
                
                if (hasInput == false)
                {
                    _stopTimer -= deltaTime;
                    if (_stopTimer <= 0)
                    {
                        Movement.TransitionToStopping();
                        return;
                    }
                }
                else
                    ResetStopTimer();
                
                Movement.IncreaseSpeed(deltaTime, direction);
            }

            private void ResetStopTimer()
            {
                _stopTimer = Data.StopThreshold;
            }
        }
        private class ChangingDirectionState : StateBase
        {
            private bool _hasChangedDirection;
            private float _direction;
            
            public override void Awake()
            {
                _hasChangedDirection = false;
                Movement.TriggerOnDirectionChanged();
                _direction = Movement.GetDirection();
            }

            public override void Execute(float deltaTime)
            {
                Movement.ChangeDirection(deltaTime, _direction);

                var speedRatio = Movement.GetSpeedRatio();
                
                if (speedRatio <= 0.09f && _hasChangedDirection == false)
                {
                    _hasChangedDirection = true;
                }
                
                if (_hasChangedDirection == false) return;

                if (speedRatio >= Data.StopChangeDirectionSpeedRatio)
                {
                    Movement.TransitionToRotating();
                }
            }
        }
        private class StoppingState : StateBase
        {
            public override void Awake()
            {
                Movement.SaveLastMovementSpeedRatio();
                Movement.TriggerOnStartStop();
                Movement.SetIsStopping(true);
            }

            public override void Execute(float deltaTime)
            {
                var hasInput = Movement.GetDirection() != 0;

                if (hasInput)
                {
                    Movement.TransitionToRotating();
                    return;
                }
                
                Movement.DecreaseSpeed(deltaTime);

                if (Movement.GetSpeedRatio() > Data.MinSpeedRatioToSnap) return;

                if (Movement.GetHasToCorrectSnap())
                {
                    Debug.Log("Correcting Snap");
                    Movement.TransitionCorrectionSnapping();
                }
                else
                {
                    Movement.TransitionSnapping();
                }
            }

            public override void Sleep()
            {
                Movement.SetIsStopping(false);
            }
        }
        private class SnappingState : StateBase
        {
            private bool _shouldInstaSnap;
            
            public override void Awake()
            {
                Movement.SetIsSnapping(true);
                _shouldInstaSnap = Movement.GetLastMovementSpeedRatio() < Data.MinSpeedRatioToSnap;
                Movement.CalculateSnapAngle(_shouldInstaSnap);
                
                if (_shouldInstaSnap)
                {
                    Movement.SnapToSlot();
                    Stop();
                }
            }

            public override void Execute(float deltaTime)
            {
                if (_shouldInstaSnap) return;
                
                Movement.SmoothSnapToSlot(deltaTime);

                if (Movement.GetIsSnapFinished())
                {
                    Stop();
                    return;
                }
                
                var hasInput = Movement.GetDirection() != 0;

                if (hasInput)
                {
                    Movement.TransitionToRotating();
                    return;
                }
                
                if (Movement.GetHasToCorrectSnap())
                {
                    Movement.TransitionCorrectionSnapping();
                    return;
                }
            }

            public override void Sleep()
            {
                Movement.SetIsSnapping(false);
            }

            private void Stop()
            {
                Movement.Stop();
                Movement.TriggerOnStopped();
                Movement.TransitionToStationary();
            }
        }

        private class CorrectionSnapping : StateBase
        {
            public override void Awake()
            {
                Movement.SetIsSnapping(true);
            }

            public override void Execute(float deltaTime)
            {
                if (Movement.GetIsCorrectionSnapFinished())
                {
                    Movement.TransitionToStationary();
                    return;
                }
                
                Movement.HandleSnapCorrection(deltaTime);
            }

            public override void Sleep()
            {
                Movement.Stop();
                Movement.SetIsSnapping(false);
                Movement.TriggerOnStopped();
                Movement.TriggerOnSnapCorrected();
                Movement.StopCorrectionSnapping();
            }
        }

        #endregion

        #region Controller
        
        private class FsmController
        {
            private readonly Dictionary<States, IState> _states = new Dictionary<States, IState>();
            private class StateData
            {
                public States StateType;
                public IState State;

                public void ChangeState(States type, IState state)
                {
                    if(state == null) return;
                    
                    Sleep();
                    StateType = type;
                    State = state;
                    Awake();
                }

                public void Awake() => State?.Awake();
                public void Sleep() => State?.Sleep();
                public void Execute(float deltaTime) => State?.Execute(deltaTime);
            }

            private StateData _currentState;
            

            public FsmController(IFsmMovement movement, IFsmMovementData movementData)
            {
                Initialize(movement, movementData);
            }
            
            public string GetCurrentState()
            {
                return (_currentState?.StateType ?? States.None).ToString();
            }

            private void Initialize(IFsmMovement movement, IFsmMovementData movementData)
            {
                _states.Add(States.Stationary, new StationaryState());
                _states.Add(States.Rotating, new RotatingState());
                _states.Add(States.ChangingDirection, new ChangingDirectionState());
                _states.Add(States.Stopping, new StoppingState());
                _states.Add(States.Snapping, new SnappingState());
                _states.Add(States.CorrectionSnapping, new CorrectionSnapping());

                foreach (var item in _states.Values.Cast<StateBase>())
                {
                    item.Initialize(movement, movementData);
                }

                _currentState = new StateData();
            }

            public void Transition(States newState)
            {
                if(newState == States.None) return;
                if(_currentState.StateType == newState) return;
                
                var newStateData = _states[newState];
                _currentState.ChangeState(newState, newStateData);
            }

            public void Execute(float deltaTime) => _currentState?.Execute(deltaTime);
        }
        
        #endregion
        
        #endregion
        
        private readonly FsmController _controller;
        private readonly IMovementData _data;
        private readonly Transform _objectToRotate;
        private readonly int _angleSlots;
        private const float MinDeltaSnapAngle = 1f;
        private float _movementStopDelayTimer;
        private float _direction;
        private float _lastDirection;
        private float _angularSpeed;
        private float _lastMovementSpeedRatio; // Is the last speed ratio before going to 'Stopping' state
        private float _angleToSnap;
        private float _targetCorrectionAngle;
        private bool _hasToCorrectSnap;
        

        public float AngularSpeed => Mathf.Abs(_angularSpeed);
        public float SpeedRatio => AngularSpeed / _data.MaxSpeed;
        public string CurrentState => _controller.GetCurrentState();
        public bool HasToCorrect => _hasToCorrectSnap;
        public bool IsSnapping { get; private set; }
        public bool IsStopping { get; private set; }

        public Vector2 Position => _objectToRotate.position;
        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action<int> OnDirectionChange;
        public event Action OnSnapCorrected;
        
        
        public MovementComponent(IMovementData data, Transform objectToRotate, int angleSlots)
        {
            _data = data;
            _objectToRotate = objectToRotate;
            _angleSlots = angleSlots;

            _controller = new FsmController(this, (IFsmMovementData)_data);
            TransitionToStationary();
        }
        
        private void RotateObject(float deltaTime)
        {
            _objectToRotate.Rotate(0f, 0f, _angularSpeed * deltaTime);
        }

        private void ClampAngularSpeed()
        {
            _angularSpeed = Mathf.Clamp(_angularSpeed, -_data.MaxSpeed, _data.MaxSpeed);
        }

        #region IMovement

        public void SetDirection(float direction)
        {
            _lastDirection = _direction;
            _direction = direction;
        }
        
        public void Update(float deltaTime)
        {
            _controller?.Execute(deltaTime);
            
            ClampAngularSpeed();
            RotateObject(deltaTime);
        }
        
        public void SetCorrectionData(int targetSlot)
        {
            _hasToCorrectSnap = true;
            _targetCorrectionAngle = GetAngleFromSlot(targetSlot);
        }

        public void ClearCorrectionData()
        {
            _hasToCorrectSnap = false;
        }

        public int GetCurrentSlot() => Mathf.RoundToInt(GetCurrentAngle() / GetSlotSize());

        #endregion

        #region Angle Slots
        
        private float GetCurrentAngle() => _objectToRotate.localEulerAngles.z;
        private float GetSlotSize() => 360f / _angleSlots;

        private float GetAngleFromSlot(int slot)
        {
            var angle = slot * GetSlotSize();
            return (angle + 360f) % 360f;
        }

        #endregion

        #region IFSMMovement
        
        // === Data === //

        #region Data
        
        public float GetSpeedRatio() => SpeedRatio;
        public float GetDirection() => _direction;
        public bool GetIsMovingAtMaxSpeed() => SpeedRatio >= 1;

        public bool GetHasLeftInput() => _movementStopDelayTimer > 0;
        public float GetLastMovementSpeedRatio() => _lastMovementSpeedRatio;
        
        public bool GetIsSnapFinished() 
            => Mathf.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _angleToSnap)) < MinDeltaSnapAngle;
        public bool GetIsCorrectionSnapFinished() 
            => Mathf.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _targetCorrectionAngle)) < MinDeltaSnapAngle;
        
        public bool GetHasToCorrectSnap() => _hasToCorrectSnap;

        #endregion
        
        // === Actions === //

        #region Actions
        
        public void Stop() => _angularSpeed = 0;

        public void ChangeDirection(float deltaTime, float direction)
        {
            _angularSpeed += (_data.ChangeDirectionAcceleration * deltaTime) * direction;
        }

        public void IncreaseSpeed(float deltaTime, float direction)
        {
            _angularSpeed += (_data.Acceleration * deltaTime) * direction;
        }

        public void DecreaseSpeed(float deltaTime)
        {
            _angularSpeed = Mathf.MoveTowards(_angularSpeed, 0, _data.Deceleration * deltaTime);
        }
        
        public void SnapToSlot()
        {
            _objectToRotate.rotation = Quaternion.Euler(0f,0f,_angleToSnap);
        }

        public void SmoothSnapToSlot(float deltaTime)
        {
            var finalSnapSpeed = _data.SnapSpeed * deltaTime;
                    
            _objectToRotate.rotation = Quaternion.Lerp( 
                _objectToRotate.rotation, 
                Quaternion.Euler(0f,0f, _angleToSnap), 
                finalSnapSpeed);
            
            if (Math.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _angleToSnap)) < MinDeltaSnapAngle)
            {
                _objectToRotate.rotation = Quaternion.Euler(0f,0f,_angleToSnap);
            }
        }

        public void SaveLastMovementSpeedRatio() 
            => _lastMovementSpeedRatio = SpeedRatio;

        public void CalculateSnapAngle(bool shouldExtraSnap)
        {
            int currentSlot = GetCurrentSlot();

            if (shouldExtraSnap)
            {
                Debug.Log("Not Enough Speed, snapping to next slot");
                currentSlot += (int)Mathf.Sign(_lastDirection);
            }

            _angleToSnap = GetAngleFromSlot(currentSlot);
        }

        public void HandleSnapCorrection(float deltaTime)
        {
            var finalSnapSpeed = _data.CorrectionSnapSpeed * deltaTime;
                    
            _objectToRotate.rotation = Quaternion.Lerp( 
                _objectToRotate.rotation, 
                Quaternion.Euler(0f,0f, _targetCorrectionAngle), 
                finalSnapSpeed);
            
            if (Math.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _targetCorrectionAngle)) < MinDeltaSnapAngle)
            {
                _objectToRotate.rotation = Quaternion.Euler(0f,0f,_targetCorrectionAngle);
            }
        }
        

        #endregion
        
        // === Action Triggers === //

        #region Action Triggers
        
        public void TriggerOnStartMoving() 
            => OnStartMoving?.Invoke((int)Mathf.Sign(_direction));

        public void TriggerOnDirectionChanged() 
            => OnDirectionChange?.Invoke((int)Mathf.Sign(_direction));

        public void TriggerOnStopped() => OnStopped?.Invoke();
        public void TriggerOnStartStop() => OnStartStop?.Invoke();
        public void TriggerOnSnapCorrected() => OnSnapCorrected?.Invoke();
        
        #endregion
        
        // === Setters === //

        #region Setters
        public void SetIsStopping(bool isStopping) => IsStopping = isStopping;
        public void SetIsSnapping(bool isSnapping) => IsSnapping = isSnapping;
        public void StopCorrectionSnapping()
        {
            _hasToCorrectSnap = false;
            _targetCorrectionAngle = -1;
        }

        #endregion
        
        // === Transitions === //
        
        #region Transitions
        
        public void TransitionToStationary() => _controller.Transition(States.Stationary);
        public void TransitionToRotating() => _controller.Transition(States.Rotating);
        public void TransitionToChangingDirection() => _controller.Transition(States.ChangingDirection);
        public void TransitionToStopping() => _controller.Transition(States.Stopping);
        public void TransitionSnapping() => _controller.Transition(States.Snapping);
        public void TransitionCorrectionSnapping() => _controller.Transition(States.CorrectionSnapping);
        
        #endregion

        #endregion

        #region Debug

        public void StopMovement()
        {
            Stop();
            TransitionToStationary();
        }

        #endregion
    }
}