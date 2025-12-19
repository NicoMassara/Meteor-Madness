using System;
using System.Collections.Generic;
using _Main.Scripts.DebugGUI;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.MyInputs;
using UnityEngine;

namespace _Main.Scripts.MyTest.Inputs
{
    public class InputMovementTest : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Transform spriteContainer;
        
        [SerializeField] private RotationData rotationData;
        private IShieldMovement _shieldMovement;


        private void Awake()
        {
            _shieldMovement = new RotationMovement(rotationData,spriteContainer);
        }

        private void Start()
        {
            var inputReader = GetComponent<InputReader>();
            inputReader.OnMovementDirectionChanged += (value) =>
            {
                _shieldMovement.SetDirection(value);
            };
        }

        private void Update()
        {
            _shieldMovement.ExecuteMovement(Time.deltaTime);
        }
    }
    
    public interface IShieldMovement
    {
        public void SetDirection(float direction);
        public void ExecuteMovement(float deltaTime);

    }

    [Serializable]
    public class RotationData
    {
        [Range(1,1000)]
        [SerializeField] private float maxAngularSpeed = 550;

        [Tooltip("Degrees per second")]
        [Range(1, 3600)] 
        [SerializeField] private float angularAcceleration = 1500f;
        
        [Tooltip("Degrees per second")]
        [Range(1,3600)] 
        [SerializeField] private float angularDeAcceleration = 2500f;

        [Range(1, 100)]
        [SerializeField] private float snapSpeed = 60;

        [Range(1, 25)] 
        [SerializeField] private float directionResponse  = 10;

        public float GetMaxAngularSpeed() => maxAngularSpeed;
        public float GetAngularAcceleration() => angularAcceleration;
        public float GetAngularDeAcceleration() => angularDeAcceleration;
        public float GetSnapSpeed() => snapSpeed;

        public float GetDirectionResponse() => directionResponse;
    }

    public class RotationMovement : IShieldMovement, RotationMovement.IRotationMovement
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
            public override void Awake()
            {
                Debug.Log("Enter Idle State");
            }

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
                Debug.Log("Enter Rotating State");
                
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
                Debug.Log("Enter Reversing State");
                
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
                Debug.Log("Enter DeAccelerating State");
                
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
                Debug.Log("Enter Snapping State");
                
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
        private readonly RotationData _data;
        private readonly Transform _objectToRotate;
        private float _direction;
        private float _angularSpeed;
        private float _snapAngle;
        private float _lastDirection;
        private bool _isStopped;
        private bool _hasReachedMaxSpeed;

        public event Action OnStartMoving;
        public event Action OnStartStop;
        public event Action OnStopped;
        public event Action OnDirectionChange;
        public event Action OnReachedMaxSpeed;
        
        public RotationMovement(RotationData data, Transform objectToRotate)
        {
            _data = data;
            _objectToRotate = objectToRotate;
            _isStopped = true;
            
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

        public void ExecuteMovement(float deltaTime)
        {
            _simpleFsm?.Execute(deltaTime);
            
            ClampAngularSpeed();
            RotateObject(deltaTime);
        }

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
                OnStartMoving?.Invoke();
        }

        public void TriggerOnDirectionChanged() => OnDirectionChange?.Invoke();
        public void TriggerOnStopped() => OnStopped?.Invoke();
        public void TriggerOnStartStop() => OnStartStop?.Invoke();
        public void Rotate(float deltaTime)
        {
            _isStopped = false;
            
            _angularSpeed += _direction * _data.GetAngularAcceleration() * deltaTime;

            if (Mathf.Abs(_angularSpeed) > _data.GetMaxAngularSpeed()
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
            float target = _data.GetMaxAngularSpeed() * _direction;
            _angularSpeed = Mathf.MoveTowards(
                _angularSpeed, 
                target,
                _data.GetAngularAcceleration() * _data.GetDirectionResponse() * deltaTime
            );
        }
        
        public void DeAccelerate(float deltaTime)
        {
            _hasReachedMaxSpeed = false;
            _angularSpeed = Mathf.MoveTowards(_angularSpeed, 
                0f, _data.GetAngularDeAcceleration() * deltaTime);
        }
        
        public void CalculateSnapAngle()
        {
            var slotSize = GetSlotSize();
            var currentAngle = GetCurrentAngle();

            int currentSlot = Mathf.RoundToInt(currentAngle / slotSize);

            if (_lastDirection > 0)
                currentSlot++;
            else if (_lastDirection < 0)
                currentSlot--;

            _snapAngle = currentSlot * slotSize;
            _snapAngle = (_snapAngle + 360f) % 360f;
        }

        public void SnapToAngle(float deltaTime)
        {
            var finalSnapSpeed = _data.GetSnapSpeed() * deltaTime;
                    
            _objectToRotate.rotation = Quaternion.Lerp( 
                _objectToRotate.rotation, 
                Quaternion.Euler(0f,0f, _snapAngle), 
                finalSnapSpeed);


            if (Math.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _snapAngle)) < 0.1f)
            {
                _objectToRotate.rotation = Quaternion.Euler(0f,0f,_snapAngle);
                _isStopped = true;
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
        private void ClampAngularSpeed() => _angularSpeed = Mathf.Clamp(_angularSpeed, -_data.GetMaxAngularSpeed(), _data.GetMaxAngularSpeed());
        private void UpdateLastDirection() => _lastDirection = _direction;

        #endregion
    }
}
