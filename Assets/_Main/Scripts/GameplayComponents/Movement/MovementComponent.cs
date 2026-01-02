using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Main.Scripts.GameplayComponents.Movement
{
    public interface IMovement
    {
        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action<int> OnDirectionChange;
        public float SpeedRatio { get; }
        public int GetCurrentSlot();
        public void SetDirection(float direction);
        public void Update(float deltaTime);
        public void MoveSlot(float direction);
    }

    public class MovementComponent : 
        IMovement,
        MovementComponent.IFsmMovement,
        MovementComponent.IMovementDebug
    {
        public interface IMovementDebug
        {
            public float AngularSpeed { get;}
            public string CurrentState { get; }

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
            
            // === Actions === //
            public void Stop();
            public void IncreaseSpeed(float deltaTime, float direction);
            public void ChangeDirection(float deltaTime, float getDirection);
            public void DecreaseSpeed(float deltaTime);
            public void SnapToSlot();
            public void SmoothSnapToSlot(float deltaTime);
            public void SaveLastMovementSpeedRatio();
            public void CalculateSnapAngle(bool shouldExtraSnap);
            
            // === Event Triggers === //
            
            public void TriggerOnStartMoving();
            public void TriggerOnDirectionChanged();
            public void TriggerOnStartStop();
            public void TriggerOnStopped();
            
            // === Transitions === //
            public void TransitionToStationary();
            public void TransitionToRotating();
            public void TransitionToChangingDirection();
            public void TransitionToStopping();
            public void TransitionSnapping();
        }
        
        #region States

        private enum States
        {
            None,
            Stationary,
            Rotating,
            ChangingDirection,
            Stopping,
            Snapping
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

                if (Movement.GetSpeedRatio() <= Data.MinSpeedRatio)
                {
                    Movement.TransitionSnapping();
                }
            }
        }
        private class SnappingState : StateBase
        {
            private bool _shouldInstaSnap;
            
            public override void Awake()
            {
                _shouldInstaSnap = Movement.GetLastMovementSpeedRatio() < Data.MinSpeedRatio;
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
            }

            private void Stop()
            {
                Movement.Stop();
                Movement.TriggerOnStopped();
                Movement.TransitionToStationary();
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
        public float AngularSpeed => Mathf.Abs(_angularSpeed);
        public float SpeedRatio => AngularSpeed / _data.MaxSpeed;
        public string CurrentState => _controller.GetCurrentState();

        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action<int> OnDirectionChange;
        
        
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
        
        public int GetCurrentSlot() => Mathf.RoundToInt(GetCurrentAngle() / GetSlotSize());

        #endregion

        #region Angle Slots
        
        public void MoveSlot(float direction)
        {
            int currentSlot = GetCurrentSlot() + (int)Mathf.Sign(direction);
            var snapAngle = currentSlot * GetSlotSize();
            snapAngle = (snapAngle + 360f) % 360f;
            
            _objectToRotate.rotation = Quaternion.Euler(0f,0f,snapAngle);
        }
        
        private float GetCurrentAngle() => _objectToRotate.localEulerAngles.z;
        private float GetSlotSize() => 360f / _angleSlots;

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

        #endregion
        
        
        // === Actions === //

        #region Actions
        
        public void Stop()
        {
            _angularSpeed = 0;
        }

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
            var snapSpeed = _data.SnapSpeed;

            if (snapSpeed == 0)
                snapSpeed = 1;
            
            var finalSnapSpeed = snapSpeed * deltaTime;
                    
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
        {
            _lastMovementSpeedRatio = SpeedRatio;
        }

        public void CalculateSnapAngle(bool shouldExtraSnap)
        {
            int currentSlot = GetCurrentSlot();

            if (shouldExtraSnap)
            {
                Debug.Log("Not Enough Speed, snapping to next slot");
                currentSlot += (int)Mathf.Sign(_lastDirection);
            }

            _angleToSnap = currentSlot * GetSlotSize();
            _angleToSnap = (_angleToSnap + 360f) % 360f;
        }

        public void TriggerOnStartMoving() 
            => OnStartMoving?.Invoke((int)Mathf.Sign(_direction));

        public void TriggerOnDirectionChanged() 
            => OnDirectionChange?.Invoke((int)Mathf.Sign(_direction));

        public void TriggerOnStopped() => OnStopped?.Invoke();
        public void TriggerOnStartStop() => OnStartStop?.Invoke();

        #endregion
        
        // === Misc === //

        #region Misc
        

        #endregion
        
        // === Transitions === //
        
        #region Transitions
        
        public void TransitionToStationary() => _controller.Transition(States.Stationary);
        public void TransitionToRotating() => _controller.Transition(States.Rotating);
        public void TransitionToChangingDirection() => _controller.Transition(States.ChangingDirection);
        public void TransitionToStopping() => _controller.Transition(States.Stopping);
        public void TransitionSnapping() => _controller.Transition(States.Snapping);
        
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