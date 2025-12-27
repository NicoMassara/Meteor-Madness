using System;
using MeteorMadness.Contracts.Interfaces;
using UnityEngine.InputSystem;

namespace _Main.Scripts.MyInputs
{
    public class KeyInput : IInput
    {
        private readonly DefaultInputs _inputsAction;
        
        public event Action<int> OnUpdateDirection;
        public event Action<bool> OnTriggerAbility;
        public event Action OnPaused;

        public KeyInput()
        {
            _inputsAction = new DefaultInputs();
        }

        public void Enable()
        {
            _inputsAction.Enable();
            
            //Rotate
            _inputsAction.Gameplay.RotateDirection.performed += OnRotate_Performed;
            _inputsAction.Gameplay.RotateDirection.canceled += OnRotate_Canceled;
            
            //Ability
            _inputsAction.Gameplay.TriggerAbility.started += OnTriggerAbility_Started;
            _inputsAction.Gameplay.TriggerAbility.canceled += OnTriggerAbility_Canceled;
            
            //Pause
            _inputsAction.Gameplay.Pause.performed += OnPause_Performed;
        }



        public void Disable()
        {
            //Rotate
            _inputsAction.Gameplay.RotateDirection.performed -= OnRotate_Performed;
            _inputsAction.Gameplay.RotateDirection.canceled -= OnRotate_Canceled;
            
            //Ability
            _inputsAction.Gameplay.TriggerAbility.performed -= OnTriggerAbility_Started;
            _inputsAction.Gameplay.TriggerAbility.canceled -= OnTriggerAbility_Canceled;
            
            //Pause
            _inputsAction.Gameplay.Pause.performed -= OnPause_Performed;
            
            _inputsAction.Disable();
        }
        
        private void OnPause_Performed(InputAction.CallbackContext input)
        {
            OnPaused?.Invoke();
        }

        #region Ability

        private void OnTriggerAbility_Started(InputAction.CallbackContext input)
        {
            OnTriggerAbility?.Invoke(true);
        }
        
        private void OnTriggerAbility_Canceled(InputAction.CallbackContext input)
        {
            OnTriggerAbility?.Invoke(false);
        }

        #endregion
        
        #region Rotation

        private void UpdateDirection(int direction)
        {
            OnUpdateDirection?.Invoke(direction);
        }

        private void OnRotate_Performed(InputAction.CallbackContext input)
        {
            UpdateDirection((int)input.ReadValue<float>());
        }
        
        private void OnRotate_Canceled(InputAction.CallbackContext input)
        {
            UpdateDirection(0);
        }

        #endregion
    }
}