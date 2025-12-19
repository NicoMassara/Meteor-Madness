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
        public float maxAngularSpeed = 100;

        [Tooltip("Degrees per second")]
        [Range(1, 3600)] 
        [SerializeField] private float angularAcceleration = 1f;
        
        [Tooltip("Degrees per second")]
        [Range(1,3600)] 
        [SerializeField] private float angularDeAcceleration = 1f;

        [Range(1, 100)]
        [SerializeField] private float snapSpeed = 10;

        [Range(1, 25)] 
        [SerializeField] private float directionResponse  = 25f;

        public float GetAngularAcceleration() => angularAcceleration;
        public float GetAngularDeAcceleration() => angularDeAcceleration;
        public float GetSnapSpeed() => snapSpeed;

        public float GetDirectionResponse() => directionResponse;
    }

    public class RotationMovement : IShieldMovement
    {
        private const int SlotCount = GameParameters.GameplayValues.AngleSlots;
        private readonly RotationData _data;
        private readonly Transform _objectToRotate;
        private float _direction;
        private float _angularSpeed;
        private float _lastAngularSpeed;
        private float _snapAngle;
        private float _lastDirection;
        private bool _isSnapping;
        private bool _isStopped;
        private bool _isRevering;

        public event Action OnStartMoving;
        public event Action OnStopped;
        public event Action OnDirectionChange;
        
        
        public RotationMovement(RotationData data, Transform objectToRotate)
        {
            _data = data;
            _objectToRotate = objectToRotate;
            _isStopped = true;
        }

        public void SetDirection(float direction)
        {
            _direction = direction;
        }

        public void ExecuteMovement(float deltaTime)
        {
            if (_direction != 0)
            {
                _isSnapping = false;
                _isStopped = false;

                if (_lastAngularSpeed == 0)
                {
                    Debug.Log("On Start Moving");
                    OnStartMoving?.Invoke();
                }

                var isReversing = _angularSpeed != 0 &&
                                  _direction != 0 &&
                                  Mathf.Sign(_direction) != Mathf.Sign(_angularSpeed);
                
                if (isReversing)
                {
                    if (!_isRevering)
                    {
                        _isRevering = true;
                        
                        Debug.Log("Direction Changed");
                        OnDirectionChange?.Invoke();
                    }
                    
                    float target = _data.maxAngularSpeed * _direction;

                    var finalAcceleration = _data.GetAngularAcceleration() * _data.GetDirectionResponse();
                    
                    _angularSpeed = Mathf.MoveTowards(
                        _angularSpeed,
                        target,
                        finalAcceleration * deltaTime
                    );
                }
                else
                {
                    _isRevering = false;
                    
                    _angularSpeed += _direction * _data.GetAngularAcceleration() * deltaTime;
                }
                
                _angularSpeed = Mathf.Clamp(_angularSpeed,  
                    -_data.maxAngularSpeed, _data.maxAngularSpeed);
                
                RotateObject(deltaTime);
            }
            else
            {
                _angularSpeed = Mathf.MoveTowards(_angularSpeed, 
                    0f, _data.GetAngularDeAcceleration() * deltaTime);
                
                RotateObject(deltaTime);

                if (Mathf.Approximately(_angularSpeed, 0f) && !_isSnapping && !_isStopped)
                {
                    _isSnapping = true;

                    var slotSize = GetSlotSize();
                    var currentAngle = GetCurrentAngle();

                    if (_lastDirection > 0)
                    {
                        _snapAngle = Mathf.Ceil(currentAngle / slotSize) * slotSize;
                    }
                    else
                    {
                        _snapAngle = Mathf.Floor(currentAngle / slotSize) * slotSize;
                    }

                    _snapAngle = (_snapAngle + 360f) % 360f;
                }


                if (_isSnapping)
                {
                    var finalSnapSpeed = _data.GetSnapSpeed() * deltaTime;
                    
                    _objectToRotate.rotation = Quaternion.Lerp( 
                        _objectToRotate.rotation, 
                        Quaternion.Euler(0f,0f, _snapAngle), 
                        finalSnapSpeed);


                    if (Math.Abs(Mathf.DeltaAngle(GetCurrentAngle(), _snapAngle)) < 0.1f)
                    {
                        _objectToRotate.rotation = Quaternion.Euler(0f,0f,_snapAngle);
                        _isSnapping = false;
                        _isStopped = true;
                        Debug.Log("Stopped");
                        OnStopped?.Invoke();
                    }
                }
            }

            _lastDirection = _direction;
            _lastAngularSpeed = _angularSpeed;
        }

        private void RotateObject(float deltaTime)
        {
            _objectToRotate.Rotate(0f, 0f, _angularSpeed * deltaTime);
        }

        private float GetCurrentAngle()
        {
            return _objectToRotate.localEulerAngles.z;
        }

        private float GetSlotSize()
        {
            return  360f / SlotCount;
        }
    }

}
