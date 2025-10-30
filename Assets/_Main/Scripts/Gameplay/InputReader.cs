using System;
using System.Collections.Generic;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;

namespace _Main.Scripts.Gameplay
{
    public class InputReader : ManagedBehavior, ILateUpdatable, IUpdatable
    {
        private const float LeftZoneWidthPercent = 0.5f;
        private DefaultInputs _inputs;
        private bool _prevBothTouched;
        private bool _rightTouched;
        private bool _leftTouched;
        private int _rotateDirection;
        private bool _areInputsEnable;
        
        public bool HasUsedAbility { get; private set; }

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Inputs;


        public UpdateGroup SelfLateUpdateGroup { get; } = UpdateGroup.Inputs;
        
        public UnityAction<int> OnMovementDirectionChanged;
        public UnityAction OnStopMovement;
        
        private void Awake()
        {
            GameManager.Instance.EventManager.Subscribe<InputsEvents.SetEnable>(EventBus_Inputs_SetEnable);
            
            _inputs = new DefaultInputs();
            
            //Pause
            _inputs.Gameplay.Pause.performed += OnPausePerformed;
        }
        
        public void ManagedUpdate()
        {
            if(_areInputsEnable == false) return;
            
            if (_rotateDirection != 0)
            {
                OnMovementDirectionChanged?.Invoke(_rotateDirection);
            }
        }
        
        public void ManagedLateUpdate()
        {
            if(_areInputsEnable == false && HasUsedAbility == false) return;
            
            HasUsedAbility = false;
        }

        private void EnableInputs()
        {
            if(_areInputsEnable == true) return;
            
            _inputs.Enable();
            
            //Rotate
            _inputs.Gameplay.RotateDirection.performed += OnRotatePerformed;
            _inputs.Gameplay.RotateDirection.canceled += OnRotateCanceled;
            
            //Ability
            _inputs.Gameplay.TriggerAbility.performed += OnTriggerAbilityPerformed;
            
            
            //Touch
            TouchSimulation.Enable();
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += OnFingerDown;
            Touch.onFingerUp += OnFingerUp;
            
            _areInputsEnable = true;
        }

        private void DisableInputs()
        {
            if(_areInputsEnable == false) return;
            
            _inputs.Disable();
            
            //Rotate
            _inputs.Gameplay.RotateDirection.performed -= OnRotatePerformed;
            _inputs.Gameplay.RotateDirection.canceled -= OnRotateCanceled;
            
            //Ability
            _inputs.Gameplay.TriggerAbility.performed -= OnTriggerAbilityPerformed;
            
            //Touch
            TouchSimulation.Disable();
            if (EnhancedTouchSupport.enabled)
            {
                Touch.onFingerDown -= OnFingerDown;
                Touch.onFingerUp -= OnFingerUp;
                EnhancedTouchSupport.Disable();
            }
            
            _areInputsEnable = false;
        }

        #region Touch Inputs

        private void OnFingerDown(Finger input)
        {
            if(IsTouchOverUI(input)) return;
            
            var pos = input.screenPosition;

            if (pos.x < Screen.width * LeftZoneWidthPercent)
            {
                _leftTouched = true;
            }
            else
            {
                _rightTouched = true;
            }

            CheckBothTouches();
            UpdateRotateDirectionFromTouch();
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
            
            UpdateRotateDirectionFromTouch();
        }

        private void UpdateRotateDirectionFromTouch()
        {
            if (_leftTouched && !_rightTouched)
            {
                UpdateDirection(1);
            }
            else if (!_leftTouched && _rightTouched)
            {
                UpdateDirection(-1);
            }
            else
            {
                UpdateDirection(0);
            }
        }

        private void CheckBothTouches()
        {
            if (_leftTouched && _rightTouched)
            {
                TriggerAbility();
            }
        }

        private bool IsTouchOverUI(Finger finger)
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = finger.screenPosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }

        #endregion

        #region Keyboard Inputs

        private void OnRotatePerformed(InputAction.CallbackContext input)
        {
            UpdateDirection((int)input.ReadValue<float>());
        }
        
        private void OnRotateCanceled(InputAction.CallbackContext input)
        {
            UpdateDirection(0);
        }
        
        private void OnTriggerAbilityPerformed(InputAction.CallbackContext input)
        {
            TriggerAbility();
        }
        
        private void OnPausePerformed(InputAction.CallbackContext input)
        {
            GameManager.Instance.EventManager.Publish(
                new GameModeEvents.SetPause{IsPaused = !GameManager.Instance.IsPaused});
        }

        #endregion
        
        private void TriggerAbility()
        {
            HasUsedAbility = true;
        }

        private void UpdateDirection(int direction)
        {
            _rotateDirection = direction;
            
            if (_rotateDirection == 0)
            {
                OnStopMovement?.Invoke();
            }
        }

        private void EventBus_Inputs_SetEnable(InputsEvents.SetEnable input)
        {
            if (input.IsEnable)
            {
                EnableInputs();
            }
            else
            {
                DisableInputs();
            }
        }
    }
}