using System;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Shield.Rotation
{
    public interface IShieldMovement
    {
        public float SpeedRatio { get; }
        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action<int> OnDirectionChange;
        public event Action OnReachedMaxSpeed;
        
        public void SetDirection(float direction);
        public void SetSpeedMultiplier(float multiplier);
        public void ExecuteMovement(float deltaTime);
        public void Restart();
        public void ForceStop();
    }
    
    public class RotationMovement : IShieldMovement, RotationMovement.IRotationMovement, IMovement
    {
        #region States

        private interface IRotationMovement
        {
            public bool GetHasInput();
            public bool GetIsSnapFinished();
            public bool GetIsReversing();
            public bool GetIsStopped();
            public void Rotate(float deltaTime);
            public void TransitionToIdle();
            public void TransitionToRotating();
            public void TransitionToReversing();
            public void TransitionToSnapping();
            public void TriggerOnStartMoving();
            public void TriggerOnDirectionChanged();
            public void TransitionToDeAccelerating();
            public void Reverse(float deltaTime);
            public void TriggerOnStartStop();
            public void TriggerOnStopped();
            public void DeAccelerate(float deltaTime);
            public void CalculateSnapAngle();
            public void SnapToAngle(float deltaTime);
        }

        private enum States
        {
            Idle,
            Rotating,
            Reversing, 
            DeAccelerating,
            Snapping 
        }

        private class StatesHolder
        {
            private readonly SimpleState _idleState;
            private readonly SimpleState _rotatingState;
            private readonly SimpleState _reversingState;
            private readonly SimpleState _snappingState;
            private readonly SimpleState _deAcceleratingState;

            public StatesHolder(IRotationMovement rotationMovement)
            {
                _idleState = new IdleState();
                _rotatingState = new RotatingState();
                _reversingState = new ReversingState();
                _snappingState = new SnappingState();
                _deAcceleratingState = new DeAcceleratingState();
                
                
                _idleState.Initialize(rotationMovement);
                _rotatingState.Initialize(rotationMovement);
                _reversingState.Initialize(rotationMovement);
                _snappingState.Initialize(rotationMovement);
                _deAcceleratingState.Initialize(rotationMovement);
            }

            public SimpleState GetSimpleState(States states)
            {
                return states switch
                {
                    States.Idle => _idleState,
                    States.Rotating => _rotatingState,
                    States.Reversing => _reversingState,
                    States.Snapping => _snappingState,
                    States.DeAccelerating => _deAcceleratingState,
                    _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
                };
            }
        }

        private sealed class SimpleFsm
        {
            private SimpleState _currentState;
            
            public event Action<SimpleState> OnStateChanged;
            
            public void Execute(float deltaTime) => _currentState?.Execute(deltaTime);
            
            public void ChangeState(SimpleState newState)
            {
                if(_currentState ==  newState)
                    return;

                if (_currentState != null)
                {
                    _currentState.Sleep();
                }
                
                _currentState = newState;
                _currentState.Awake();
                OnStateChanged?.Invoke(_currentState);
            }
        }

        #region States
        
        private abstract class SimpleState
        {
            protected IRotationMovement Rotation { get; private set; }

            public void Initialize(IRotationMovement rotation)
            {
                Rotation = rotation;
            }

            public virtual void Awake() {}
            public virtual void Execute(float deltaTime) {}
            public virtual void Sleep() {}
        }
        private class IdleState : SimpleState
        {
            public override void Execute(float deltaTime)
            {
                if (Rotation.GetHasInput())
                {
                    Rotation.TransitionToRotating();
                }
            }
        }
        private class RotatingState : SimpleState
        {
            private const float NoInputGraceTime = 0.06f;
            private float _noInputTimer = NoInputGraceTime;
            
            public override void Awake()
            {
                Rotation.TriggerOnStartMoving();
                _noInputTimer = NoInputGraceTime;
            }

            public override void Execute(float deltaTime)
            {
                Rotation.Rotate(deltaTime);

                if (Rotation.GetIsReversing())
                {
                    Rotation.TransitionToReversing();
                    return;
                }

                if (Rotation.GetHasInput())
                {
                    _noInputTimer = NoInputGraceTime;
                    return;
                }

                _noInputTimer -= deltaTime;

                if (_noInputTimer <= 0f)
                {
                    Rotation.TransitionToDeAccelerating();
                }
            }
        }
        private class ReversingState : SimpleState
        {
            public override void Awake()
            {
                Rotation.TriggerOnDirectionChanged();
            }

            public override void Execute(float deltaTime)
            {
                Rotation.Reverse(deltaTime);
                
                if (Rotation.GetIsReversing() == false)
                {
                    Rotation.TransitionToRotating();
                }
            }
        }
        private class DeAcceleratingState : SimpleState
        {
            public override void Awake()
            {
                Rotation.TriggerOnStartStop();
            }

            public override void Execute(float deltaTime)
            {
                Rotation.DeAccelerate(deltaTime);
                
                if (Rotation.GetIsStopped())
                {
                    Rotation.TransitionToSnapping();
                }
            }
        }
        private class SnappingState : SimpleState
        {
            public override void Awake()
            {
                Rotation.CalculateSnapAngle();
            }

            public override void Execute(float deltaTime)
            {
                Rotation.SnapToAngle(deltaTime);
                
                if (Rotation.GetHasInput())
                {
                    Rotation.TransitionToRotating();
                    return;
                }
                 
                if (Rotation.GetIsSnapFinished())
                {
                    Rotation.TransitionToIdle();
                }
            }

            public override void Sleep()
            {
                Rotation.TriggerOnStopped();
            }
        }
        
        #endregion

        #endregion
        
        private const int SlotCount = GameParameters.GameplayValues.AngleSlots;
        private SimpleFsm _simpleFsm;
        private StatesHolder _statesHolder;
        private readonly RotationDataSo _data;
        private readonly Transform _objectToRotate;
        private float _speedMultiplier;
        private float _direction;
        private float _angularSpeed;
        private float _snapAngle;
        private float _lastDirection;
        private bool _hasReachedMaxSpeed;
        private int _currentSlot;

        public float SpeedRatio => Mathf.Abs(_angularSpeed) / _data.MaxAngularSpeed;

        public event Action<int> OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action<int> OnDirectionChange;
        public event Action OnReachedMaxSpeed;
        
        public RotationMovement(RotationDataSo data, Transform objectToRotate)
        {
            _data = data;
            _objectToRotate = objectToRotate;
            _speedMultiplier = 1;
            
            InitializeFsm();
        }

        private void InitializeFsm()
        {
            _statesHolder = new StatesHolder(this);
            _simpleFsm = new SimpleFsm();
            
            TransitionToIdle();
        }

        #region IShieldMovement
        
        public void SetDirection(float direction)
        {
            _direction = direction;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMultiplier = Mathf.Clamp(multiplier, 0, Mathf.Infinity);
        }

        public void ExecuteMovement(float deltaTime)
        {
            _simpleFsm?.Execute(deltaTime);
            
            ClampAngularSpeed();
            RotateObject(deltaTime);
        }

        public void Restart()
        {
            _angularSpeed = 0;
            _objectToRotate.rotation = Quaternion.Euler(0, 0, 0);
        }

        public void ForceStop()
        {
            _angularSpeed = 0;
            _direction = 0;
        }

        public Vector2 GetPosition() => _objectToRotate.position;
        public int GetCurrentSlot() => Mathf.RoundToInt(GetCurrentAngle() / GetSlotSize());

        #endregion
        
        #region IRotationMovement
        
        public bool GetHasInput() => _direction != 0;
        public bool GetIsSnapFinished() => Mathf.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _snapAngle)) < 0.1f;
        public bool GetIsStopped() => Mathf.Approximately(_angularSpeed, 0f);
        public bool GetIsReversing()
        {
            return GetHasInput() &&
                   GetIsMoving() &&
                   Mathf.Sign(_direction) != Mathf.Sign(_angularSpeed);
        }
        
        public void TriggerOnStartMoving()
        {
            if(GetIsStopped())
                OnStartMoving?.Invoke((int)Mathf.Sign(_direction));
        }

        public void TriggerOnDirectionChanged() => OnDirectionChange?.Invoke((int)Mathf.Sign(_direction));
        public void TriggerOnStopped() => OnStopped?.Invoke();
        public void TriggerOnStartStop() => OnStartStop?.Invoke();
        public void Rotate(float deltaTime)
        {
            float targetSpeed = (_direction * _data.MaxAngularSpeed) * _speedMultiplier;

            _angularSpeed = Mathf.MoveTowards(
                _angularSpeed,
                targetSpeed,
                _data.AngularAcceleration * deltaTime
            );

            if (Mathf.Abs(_angularSpeed) > _data.MaxAngularSpeed
                && _hasReachedMaxSpeed == false)
            {
                OnReachedMaxSpeed?.Invoke();
                _hasReachedMaxSpeed = true;
            }
            
            UpdateLastDirection();
        }
        public void Reverse(float deltaTime)
        {
            _hasReachedMaxSpeed = false;
            float target = _data.MaxAngularSpeed * _direction;
            _angularSpeed = Mathf.MoveTowards(
                _angularSpeed, 
                target,
                _data.AngularAcceleration * _data.DirectionResponse * deltaTime
            );
            
            Debug.Log($"Speed Ratio: {SpeedRatio}");
        }
        public void DeAccelerate(float deltaTime)
        {
            _hasReachedMaxSpeed = false;
            _angularSpeed = Mathf.MoveTowards(_angularSpeed, 
                0f, _data.AngularDeAcceleration * deltaTime);
        }
        public void CalculateSnapAngle()
        {
            int currentSlot = GetCurrentSlot();

            if (_lastDirection > 0)
                currentSlot++;
            else if (_lastDirection < 0)
                currentSlot--;

            _snapAngle = currentSlot * GetSlotSize();
            _snapAngle = (_snapAngle + 360f) % 360f;
        }
        public void SnapToAngle(float deltaTime)
        {
            var finalSnapSpeed = _data.SnapSpeed * deltaTime;
                    
            _objectToRotate.rotation = Quaternion.Lerp( 
                _objectToRotate.rotation, 
                Quaternion.Euler(0f,0f, _snapAngle), 
                finalSnapSpeed);


            if (Math.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _snapAngle)) < 0.1f)
            {
                _objectToRotate.rotation = Quaternion.Euler(0f,0f,_snapAngle);
            }
        }

        public void TransitionToIdle() => ChangeState(States.Idle);
        public void TransitionToRotating() => ChangeState(States.Rotating);
        public void TransitionToReversing() => ChangeState(States.Reversing);
        public void TransitionToSnapping() => ChangeState(States.Snapping);
        public void TransitionToDeAccelerating() => ChangeState(States.DeAccelerating);

        #endregion
        
        #region Private Methods
        public bool GetIsMoving() => !Mathf.Approximately(_angularSpeed, 0f);
        private void RotateObject(float deltaTime) => _objectToRotate.Rotate(0f, 0f, _angularSpeed * deltaTime);
        private float GetCurrentAngle() => _objectToRotate.localEulerAngles.z;
        private float GetSlotSize() => 360f / SlotCount;
        private void ChangeState(States newState) => _simpleFsm.ChangeState(_statesHolder.GetSimpleState(newState));
        private void ClampAngularSpeed()
        {
            _angularSpeed = Mathf.Clamp(_angularSpeed, 
                -_data.MaxAngularSpeed * _speedMultiplier
                , _data.MaxAngularSpeed * _speedMultiplier);
        }
        private void UpdateLastDirection() => _lastDirection = _direction;

        #endregion
    }
}