using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace _Main.Scripts.Inputs
{
    public class TouchInput
    {
        private const float AngleOffset = 180f;
        private readonly IInputData _inputData;
        private readonly Camera _mainCamera;
        private bool _isPressingDown;
        private Vector2 _touchPosition;
        private bool _hasTouchAngle;
        private float _touchAngle;
        private float _lastTouchAngle;
        
        public event Action<float> OnDirectionChanged;
        public event Action<Vector2> OnPressingToMove;
        public event Action OnReleasedToMove;
        
        public TouchInput(IInputData inputData, Camera mainCamera)
        {
            _inputData = inputData;
            _mainCamera = mainCamera;
        }
        
        public void Initialize()
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();

            Touch.onFingerDown += OnFinderDownHandler;
            Touch.onFingerUp += OnFingerUpHandler;
            Touch.onFingerMove += OnFingerMoveHandler;
        }

        #region Gette

        private bool GetIsInsideTouchZone(Vector2 touchPosition)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
                _inputData.LimitedZone,
                touchPosition,
                null);
        }

        private bool TrySetTouchAngle(Vector2 touchPos, out float touchAngle)
        {
            touchAngle = -1;
            var temp = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _inputData.LimitedZone,
                touchPos,
                null,
                out var localPoint);

            if (temp)
            {
                if (localPoint.magnitude < _inputData.DeadZoneRadius) 
                    return false;
            
                float angle = (Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg) + 180f;
                if (angle < 0)
                    angle += 360f;
            
                touchAngle = angle;
            }

            return temp;
        }

        #endregion
        
        #region Handlers

        private void OnFingerUpHandler(Finger input)
        {
            _isPressingDown = false;
            _hasTouchAngle = false;
            OnReleasedToMove?.Invoke();
        }

        private void OnFinderDownHandler(Finger input)
        {
            _isPressingDown = GetIsInsideTouchZone(input.screenPosition);
            
            if (_isPressingDown == false)
            {
                return; 
            }
            
            OnPressingToMove?.Invoke(input.screenPosition);
            
            _hasTouchAngle = TrySetTouchAngle(input.screenPosition, out float touchAngle);
            
            if (_hasTouchAngle)
            {
                _touchAngle = touchAngle;
                OnDirectionChanged?.Invoke(_touchAngle);
            }
            else
            {
                //Debug.Log("Inside Dead Zone");
            }
        }
        
        private void OnFingerMoveHandler(Finger input)
        {
            if(_isPressingDown == false) return;
            
            _lastTouchAngle = _touchAngle;
            
            _hasTouchAngle = TrySetTouchAngle(input.screenPosition, out float touchAngle);
            
            if (_hasTouchAngle)
            {
                _touchAngle = touchAngle;

                if (!Mathf.Approximately(_lastTouchAngle, _touchAngle))
                {
                    OnDirectionChanged?.Invoke(_touchAngle);
                }
            }
            else
            {
                //Debug.Log("Inside Dead Zone");
            }
        }

        #endregion
    }
}