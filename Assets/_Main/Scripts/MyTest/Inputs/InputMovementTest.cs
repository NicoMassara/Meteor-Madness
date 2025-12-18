using System;
using _Main.Scripts.MyInputs;
using UnityEngine;

namespace _Main.Scripts.MyTest.Inputs
{
    public class InputMovementTest : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Transform spriteContainer;

        [Serializable]
        private class MovementData
        {
            [Range(1,50)]
            [SerializeField] private float rotateSpeed = 180f;
            [Range(1,50)]
            public float SnapSpeed = 10f;
            public float GetRotateSpeed() => rotateSpeed * 10;
        }
        [SerializeField] private MovementData movementData;

        private class NewShieldMovement
        {
            private const int TotalSlots = GameParameters.GameplayValues.AngleSlots;
            public int Direction { get; set; }
            private readonly Transform _objectToRotate;
            private readonly MovementData _movementData;
            private int _currentSlot = 0;
            
            private float _currentAngle;
            private float _targetAngle;
            private bool _snapping;
            
            public NewShieldMovement(Transform objectToRotate, MovementData movementData)
            {
                _objectToRotate = objectToRotate;
                _movementData = movementData;
            }

            public void Execute(float deltaTime)
            {
                if (Mathf.Abs(Direction) > 0.01f)
                {
                    _currentAngle += Direction * GetFinalSpeed() * deltaTime;
                    _snapping = false;
                }
                else if (!_snapping)
                {
                    float step = 360f / TotalSlots;
                    _targetAngle = Mathf.Round(_currentAngle / step) * step;
                    _snapping = true;
                }

                if (_snapping)
                {
                    _currentAngle = Mathf.LerpAngle(
                        _currentAngle,
                        _targetAngle,
                        deltaTime * _movementData.SnapSpeed
                    );

                    if (Mathf.Abs(Mathf.DeltaAngle(_currentAngle, _targetAngle)) < 0.1f)
                    {
                        _currentAngle = _targetAngle;
                        _snapping = false;
                    }
                }
                
                _objectToRotate.rotation = Quaternion.Euler(0f, 0f, _currentAngle);
            }

            private float GetFinalSpeed()
            {
                return _movementData.GetRotateSpeed();
            }
        }
        
        private NewShieldMovement _shieldMovement;


        private void Awake()
        {
            _shieldMovement = new NewShieldMovement(spriteContainer, movementData);
        }
        
        private void Start()
        {
            var inputReader = GetComponent<InputReader>();
            inputReader.OnMovementDirectionChanged += (value) =>
            {
                _shieldMovement.Direction = value;
            };
        }

        private void Update()
        {
            _shieldMovement.Execute(Time.deltaTime);
        }
    }
}