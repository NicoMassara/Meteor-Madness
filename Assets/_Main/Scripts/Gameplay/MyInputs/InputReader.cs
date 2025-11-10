using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine.InputSystem;
using System;

namespace _Main.Scripts.Gameplay.MyInputs
{
    public class InputReader : ManagedBehavior, IInputReader, ILateUpdatable, IUpdatable
    {
        private DefaultInputs _inputs;
        private ITouchInputReader _touchInput;
        private int _rotateDirection;
        private bool _areInputsEnable;
        
        public bool HasUsedAbility { get; private set; }

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Inputs;
        public TickGroup SelfTickGroup { get; } = TickGroup.FullTick;
        public float LastUpdateTime { get; set; }

        public UpdateGroup SelfLateUpdateGroup { get; } = UpdateGroup.Inputs;
        
        public event Action<int> OnMovementDirectionChanged;
        public event Action OnStopMovement;
        public event Action<bool> OnAbilityTriggered;
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private InputsDebugData _debugData;
#endif
        
        private void Awake()
        {
            GameEventCaller.Subscribe<InputsEvents.SetEnable>(EventBus_Inputs_SetEnable);
            GameManager.Instance.SetInputReader(this);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData = new InputsDebugData();
#endif
                
#if UNITY_STANDALONE || UNITY_EDITOR
            _inputs = new DefaultInputs();
            
            //Pause
            _inputs.Gameplay.Pause.performed += OnPause_Performed;
#endif
#if UNITY_ANDROID || UNITY_IOS
            _touchInput = new TouchInputReader();
#endif
        }
        
        public void ExecuteUpdate()
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
            _inputs.Gameplay.RotateDirection.performed += OnRotate_Performed;
            _inputs.Gameplay.RotateDirection.canceled += OnRotate_Canceled;
            
            //Ability
            _inputs.Gameplay.TriggerAbility.performed += OnTriggerAbility_Performed;
            _inputs.Gameplay.TriggerAbility.canceled += OnTriggerAbility_Canceled;
#endif

#if UNITY_ANDROID || UNITY_IOS
            _touchInput.Enable();
            
            _touchInput.OnTriggerAbility += TriggerAbility;
            _touchInput.OnUpdateDirection += UpdateDirection;
#endif
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.HorizontalAxis = 0;
            
#endif

            _areInputsEnable = true;
        }

        private void DisableInputs()
        {
            if(_areInputsEnable == false) return;
            
#if UNITY_STANDALONE || UNITY_EDITOR
            _inputs.Disable();
            
            //Rotate
            _inputs.Gameplay.RotateDirection.performed -= OnRotate_Performed;
            _inputs.Gameplay.RotateDirection.canceled -= OnRotate_Canceled;
            
            //Ability
            _inputs.Gameplay.TriggerAbility.performed -= OnTriggerAbility_Performed;
#endif

#if UNITY_ANDROID || UNITY_IOS
            _touchInput.OnTriggerAbility -= TriggerAbility;
            _touchInput.OnUpdateDirection -= UpdateDirection;
            
            _touchInput.Disable();
#endif
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.HorizontalAxis = 0;
            
#endif
            
            _areInputsEnable = false;
        }
        
        #region Keyboard Inputs

        private void OnRotate_Performed(InputAction.CallbackContext input)
        {
            UpdateDirection((int)input.ReadValue<float>());
        }
        
        private void OnRotate_Canceled(InputAction.CallbackContext input)
        {
            UpdateDirection(0);
        }
        
        private void OnTriggerAbility_Performed(InputAction.CallbackContext input)
        {
            TriggerAbility(true);
        }
        
        private void OnTriggerAbility_Canceled(InputAction.CallbackContext input)
        {
            TriggerAbility(false);
        }
        
        private void OnPause_Performed(InputAction.CallbackContext input)
        {
            GameManager.Instance.EventManager.Publish(
                new GameModeEvents.SetPause{IsPaused = !GameManager.Instance.IsPaused});
        }

        #endregion
        
        private void TriggerAbility(bool isActive)
        {
            HasUsedAbility = isActive;
            OnAbilityTriggered?.Invoke(isActive);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.TriggerAbility = HasUsedAbility;
            
#endif
        }

        private void UpdateDirection(int direction)
        {
            _rotateDirection = direction;
            
            if (_rotateDirection == 0)
            {
                OnStopMovement?.Invoke();
            }
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.HorizontalAxis = _rotateDirection;
            
#endif
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