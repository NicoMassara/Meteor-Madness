using System;
using UnityEngine;

namespace _Main.Scripts.Inputs
{
    public abstract class DeviceInput<T> : IDeviceInput, IDebugDeviceInput
    where T : IInputData
    {
        private readonly T _inputData;
        private readonly Camera _mainCamera;
        private const float AngleOffset = 180f;
        private readonly InputRadius _inputRadius;
        private Vector2 _pressPosition;
        private bool _wasPressingToMove;
        
        protected T InputData => _inputData;

        #region IDebugDeviceInput

        public Vector2 PressPosition => _pressPosition;
        public Vector2 InputPosition { get; private set; }
        public float CurrentAngle { get; private set; }
        public float LastAngle { get; private set; }
        public float InputMagnitude { get; private set; }
        public bool IsPressingToMove { get; private set; }
        public float DeadZone => _inputData.DeadZoneRadius;
        public float SafeZone => _inputData.SafeZoneRadius;
        public RectTransform LimitedZone => _inputData.LimitedZone;

        #endregion

        public event Action<float> OnDirectionChanged;
        public event Action<float> OnMagnitudeChanged;
        public event Action<Vector2> OnPressingToMove;
        public event Action OnReleasedToMove;


        protected DeviceInput(T inputData, Camera mainCamera)
        {
            _inputData = inputData;
            _mainCamera = mainCamera;
            _inputRadius = new InputRadius(AngleOffset);
            _inputRadius.OnDirectionChanged += InputRadius_OnDirectionChangedHandler;
            _inputRadius.OnMagnitudeChanged += InputRadius_OnMagnitudeChangedHandler;
            OnPressingToMove += OnPressingToMoveHandler;
            OnReleasedToMove += OnReleasedToMoveHandler;
        }

        public virtual void Execute()
        {
            CheckForMoveInput();
        }

        private void CheckForMoveInput()
        {
            // Checks for input
            if (GetIsPressingToMove())
            {
                // Stores press position
                // If it was not pressing, triggers the event
                if (_wasPressingToMove == false)
                {
                    _wasPressingToMove = true;
                    OnPressingToMove?.Invoke(GetInputPressPosition());
                }
                // The next frame after pressing, it'll start to update the direction
                else 
                {
                    var inputPosition = GetInputPosition();
                    var inputDelta = inputPosition - _pressPosition;
                    
                    InputPosition = inputPosition;
                    
                    var pressPositionInWorld = GetPositionInWorld(_pressPosition);
                    var inputPositionInWorld = GetPositionInWorld(inputPosition);

                    if (GetIsOutOfDeadZone(pressPositionInWorld, inputPositionInWorld))
                    {
                        _inputRadius.SetAngleFromDirection(inputDelta);
                        
                        var magnitude = GetSafeZoneMagnitude(pressPositionInWorld, inputPositionInWorld);
                        _inputRadius.SetMagnitude(magnitude);
                    }
                    else
                    {
                        _inputRadius.SetMagnitude(0);
                    }
                }
            }
            // Triggers release when stop pressing
            else if (_wasPressingToMove)
            {
                _wasPressingToMove = false;
                _inputRadius.ClearAngle();
                OnReleasedToMove?.Invoke();
            }
        }

        /// <summary>
        /// Returns the current state of input
        /// </summary>
        /// <returns>TRUE - Input is being pressed</returns>
        protected abstract bool GetIsPressingToMove();
        /// <summary>
        /// Returns the press input position on screen
        /// </summary>
        /// <returns>Vector2 of input on screen</returns>
        protected abstract Vector2 GetInputPressPosition();
        /// <summary>
        /// Returns the current input position on screen
        /// </summary>
        /// <returns>Vector2 of input on screen</returns>
        protected abstract Vector2 GetInputPosition();

        protected Vector3 GetPositionInWorld(Vector2 screenPosition)
        {
            var cameraDepth = -10;
            return _mainCamera.ScreenToWorldPoint(
                new Vector3(screenPosition.x, screenPosition.y, cameraDepth)
            );
        }
        protected bool GetIsOutOfDeadZone(Vector2 pressPosition, Vector2 inputPosition)
        {
            var distance = Vector2.Distance(pressPosition, inputPosition);
            
            return distance > _inputData.DeadZoneRadius;
        }
        protected float GetSafeZoneMagnitude(Vector2 pressPosition, Vector2 inputPosition)
        {
            float distance = Vector2.Distance(pressPosition, inputPosition);

            float magnitude = Mathf.Clamp01(
                (distance - _inputData.DeadZoneRadius) /
                (_inputData.SafeZoneRadius - _inputData.DeadZoneRadius)
            );

            return magnitude;
        }

        protected virtual void InputRadius_OnDirectionChangedHandler(float input)
        {
            CurrentAngle = input;
            OnDirectionChanged?.Invoke(input);
        }
        
        protected virtual void InputRadius_OnMagnitudeChangedHandler(float input)
        {
            InputMagnitude = input;
            OnMagnitudeChanged?.Invoke(input);
        }
        
        protected virtual void OnPressingToMoveHandler(Vector2 position)
        {
            _pressPosition = position;
            IsPressingToMove = true;
        }
        
        protected virtual void OnReleasedToMoveHandler()
        {
            IsPressingToMove = false;
        }
    }
}