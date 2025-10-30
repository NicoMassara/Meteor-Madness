using _Main.Scripts.Managers.UpdateManager;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Action = System.Action;
using Screen = UnityEngine.Device.Screen;

namespace _Main.Scripts.MyInputs
{
    public class InputHandler : ManagedBehavior
    {
        private const float LeftZoneWidthPercent = 0.5f;
        private DefaultInputs _inputs;
        private bool _prevBothTouched;
        private bool _rightTouched;
        private bool _leftTouched;
        private float _rotateDirection;

        public event Action OnAbilityTriggered;
        public event Action OnPause;

        private void Awake()
        {
            _inputs = new DefaultInputs();
            

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _inputs.Enable();
            EnhancedTouchSupport.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _inputs.Disable();
            EnhancedTouchSupport.Disable();
        }

        #region Touch Inputs

        private void OnFingerDown(Finger input)
        {
            var pos = input.screenPosition;

            if (pos.x < Screen.width * LeftZoneWidthPercent)
            {
                _leftTouched = true;
            }
            else
            {
                _rightTouched = true;
            }
        }
        
        private void OnFingerUp(Finger input)
        {
            var pos = input.screenPosition;

            if (pos.x < Screen.width * LeftZoneWidthPercent)
            {
                _leftTouched = false;
            }
            else
            {
                _rightTouched = false;
            }
        }

        private void UpdateRotateDirectionFromTouch()
        {
            if (_leftTouched && !_rightTouched)
            {
                _rotateDirection = -1f;
            }
            else if (!_leftTouched && _rightTouched)
            {
                _rotateDirection = 1f;
            }
            else
            {
                _rotateDirection = 0f;
            }
        }

        private void CheckBothTouches()
        {
            if (_leftTouched && _rightTouched)
            {
                TriggerAbility();
            }
        }

        #endregion

        #region Keyboard Inputs

        private void OnRotatePerformed(InputAction.CallbackContext input)
        {
            _rotateDirection = input.ReadValue<float>();
        }
        
        private void OnRotateCanceled(InputAction.CallbackContext input)
        {
            _rotateDirection = 0;
        }
        
        private void OnTriggerAbilityPerformed(InputAction.CallbackContext input)
        {
            TriggerAbility();
        }
        
        private void OnPausePerformed(InputAction.CallbackContext input)
        {
            OnPause?.Invoke();
        }

        #endregion


        private void TriggerAbility()
        {
            OnAbilityTriggered?.Invoke();
        }

        private void UpdateDirection(float direction)
        {
            _rotateDirection = direction;

            if (_rotateDirection != 0)
            {
                
            }
        }

    }
}