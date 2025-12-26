using System;
using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;

namespace _Main.Scripts.MyInputs
{
    public class InputReader : ManagedBehavior, IInputReader, IUpdatable
    {
        private IInput input;

        private int _rotateDirection;
        private bool _areInputsEnable;
        private bool _wasDisabled;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Inputs;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;
        
        public event Action<int> OnMovementDirectionChanged;
        public event Action<bool> OnAbilityTriggered;
        
        
        private void Awake()
        {
            InputsEventSubscriber.SetEnable(EventBus_Inputs_SetEnable);
            GameManager.Instance.SetInputReader(this);
        }

        private void Start()
        {
            
#if UNITY_ANDROID || UNITY_IOS
            input = new TouchInput();
#else
            input = new KeyInput();
#endif
            input.Enable();
            input.OnTriggerAbility += TriggerAbility;
            input.OnUpdateDirection += UpdateDirection;
            input.OnPaused += TriggerPause;
            EnableInputs();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            if (_areInputsEnable == false && _wasDisabled == false)
            {
                _wasDisabled = true;
                OnMovementDirectionChanged?.Invoke(0);
            }
            else if (_areInputsEnable)
            {
                OnMovementDirectionChanged?.Invoke(_rotateDirection);
            }
        }

        public void EnableInputs()
        {
            if(_areInputsEnable == true) return;
            

            _areInputsEnable = true;
        }

        private void DisableInputs()
        {
            if(_areInputsEnable == false) return;
            
            
            _areInputsEnable = false;
            _wasDisabled = false;
        }

        #region Actions

        private void TriggerPause()
        {
            GameManager.Instance.EventManager.Publish(
                new GameModeEvents.SetPause{IsPaused = !GameManager.Instance.IsPaused});
        }
        
        private void TriggerAbility(bool isPressed)
        {
            if(_areInputsEnable == false) return;
            
            OnAbilityTriggered?.Invoke(isPressed);
        }

        private void UpdateDirection(int direction)
        {
            _rotateDirection = direction;
        }

        #endregion
        


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