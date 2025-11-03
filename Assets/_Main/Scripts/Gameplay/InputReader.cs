using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
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
        private DefaultInputs _inputs;
        private int _rotateDirection;
        private bool _areInputsEnable;
        private ITouchInputReader _touchInput;
        
        public bool HasUsedAbility { get; private set; }

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Inputs;


        public UpdateGroup SelfLateUpdateGroup { get; } = UpdateGroup.Inputs;
        
        public UnityAction<int> OnMovementDirectionChanged;
        public UnityAction OnStopMovement;
        
        private void Awake()
        {
            GameManager.Instance.EventManager.Subscribe<InputsEvents.SetEnable>(EventBus_Inputs_SetEnable);
            
#if UNITY_STANDALONE || UNITY_EDITOR
            _inputs = new DefaultInputs();
            
            //Pause
            _inputs.Gameplay.Pause.performed += OnPausePerformed;
#endif
#if UNITY_ANDROID || UNITY_IOS
            _touchInput = new TouchInputReader();
#endif
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

#if UNITY_STANDALONE || UNITY_EDITOR
            _inputs.Enable();
            //Rotate
            _inputs.Gameplay.RotateDirection.performed += OnRotatePerformed;
            _inputs.Gameplay.RotateDirection.canceled += OnRotateCanceled;
            
            //Ability
            _inputs.Gameplay.TriggerAbility.performed += OnTriggerAbilityPerformed;
#endif

#if UNITY_ANDROID || UNITY_IOS
            _touchInput.Enable();
            
            _touchInput.OnTriggerAbility += TriggerAbility;
            _touchInput.OnUpdateDirection += UpdateDirection;
#endif
            

            _areInputsEnable = true;
        }

        private void DisableInputs()
        {
            if(_areInputsEnable == false) return;
            
#if UNITY_STANDALONE || UNITY_EDITOR
            _inputs.Disable();
            
            //Rotate
            _inputs.Gameplay.RotateDirection.performed -= OnRotatePerformed;
            _inputs.Gameplay.RotateDirection.canceled -= OnRotateCanceled;
            
            //Ability
            _inputs.Gameplay.TriggerAbility.performed -= OnTriggerAbilityPerformed;
#endif

#if UNITY_ANDROID || UNITY_IOS
            _touchInput.OnTriggerAbility -= TriggerAbility;
            _touchInput.OnUpdateDirection -= UpdateDirection;
            
            _touchInput.Disable();
#endif
            
            _areInputsEnable = false;
        }

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
                UpdateDirection(0);
                DisableInputs();
            }
        }
    }
}