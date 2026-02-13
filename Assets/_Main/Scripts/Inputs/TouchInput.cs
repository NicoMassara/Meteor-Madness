using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace _Main.Scripts.Inputs
{
    public class TouchInput : DeviceInput<ITouchInputData>
    {
        private bool _isPressingDown;
        private Vector2 _touchPosition;
        private Vector2 _touchPressPosition;
        
        public TouchInput(ITouchInputData inputData, Camera mainCamera)
            : base(inputData, mainCamera)
        {
            Initialize();
        }

        private void Initialize()
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();

            Touch.onFingerDown += OnFinderDownHandler;
            Touch.onFingerUp += OnFingerUpHandler;
            Touch.onFingerMove += OnFingerMoveHandler;
        }

        public override void Execute()
        {
            base.Execute();
        }
        
        private bool IsInsideLimitedZone(Vector2 screenPos)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
                InputData.LimitedZone,
                screenPos,
                Camera.main
            );
        }

        #region Override
        
        protected override bool GetIsPressingToMove() => _isPressingDown;
        protected override Vector2 GetInputPressPosition() => _touchPressPosition;
        protected override Vector2 GetInputPosition() => _touchPosition;
        
        #endregion

        #region Handlers

        private void OnFingerUpHandler(Finger input)
        {
            _isPressingDown = false; 
        }

        private void OnFinderDownHandler(Finger input)
        {
            var screenPos = input.screenPosition;
            
            if(IsInsideLimitedZone(screenPos))
            {
                _touchPressPosition = input.screenPosition;
                _touchPosition = input.screenPosition;
                _isPressingDown = true;  
            }
            
        }
        
        private void OnFingerMoveHandler(Finger input)
        {
            if(_isPressingDown == false) return;
            
            _touchPosition = input.screenPosition;
        }

        #endregion
    }
}